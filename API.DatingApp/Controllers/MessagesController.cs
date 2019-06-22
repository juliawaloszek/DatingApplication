using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DatingApp.Data;
using API.DatingApp.Dtos;
using API.DatingApp.Helpers;
using API.DatingApp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.DatingApp.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]

    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage (int userId, int messageId)
        {
            //Sprawdzenie czy użytkownik istnieje
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //pobranie wiadomości z bazy 
            var messageFromRepo = await _repo.GetMessage(messageId);

            if(messageFromRepo == null) 
                return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, 
            [FromQuery]MessageParams messageParams)
        //zwrócenie listy wiadomości danego użytkownika
        {
            //Sprawdzenie czy użytkownik istnieje
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();   

            //Dodanie propertki do messageParams
            messageParams.UserId = userId;
            
            //Przypisanie do messageFromRepo lity wiadomości z bazy 
            var messagesFromRepo = await _repo.GetMessageForUser(messageParams);

            //mapowanie DTO 
            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            //Dodanie pagination do zwracanej wiadomości
            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, 
                messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            //Sprawdzenie czy użytkownik istnieje
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //Pobranie listy wiadomości z bazy 
            var messageFromRepo = await _repo.GetMessageThread(userId, recipientId);

            //przekształcenie messageFromRepo => MessageToReturnDto
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

            return Ok(messageThread);
        }


        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            //pobranie z repozytorium id wysyłającego
            var sender = await _repo.GetUser(userId);

            //Sprawdzenie czy użytkownik istnieje
            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //ustawienie SenderId w wiadomości na aktualnego użytkownika
            messageForCreationDto.SenderId = userId;
            
            //oraz odbiorcy wiadomości 
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

            //czy odbiorca istnieje
            if(recipient == null)
            {
                return BadRequest("Could not find user");
            }

            //zmapowanie DTO Wiadomości z klasą Wiadomości 
            //przekształcenie messageForCreationDto => Message
            var message = _mapper.Map<Message>(messageForCreationDto);

            _repo.Add(message);

            
            if(await _repo.SaveAllAsync())
            {
                //przekształcenie Message => messageForCreationDto
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);

                //CreatedAtRoute(Name of root/nazwa ścieżki, objekt do przekazania)
                return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);
            }
                

            throw new Exception("Creating the messsafe failed on save");
        }

        //metoda usuwania wiadomości. Wykorzystujemy metode POST a nie DELETE gdyż 
        // damy użytkownikom możliwość akceptacji usuniecia wiadomości
        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            //Sprawdzenie czy użytkownik istnieje
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            //pobranie wiadomości z bazy
            var messageFromRepo = await _repo.GetMessage(id);

            //ustawienia parametru iż wiadomosć jest usunieta
            if(messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }

            if(messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.RecipientDeleted = true;
            }

            //usuń jesli obie osoby potwierdziły usuniecie
            if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
            {
                _repo.Delete(messageFromRepo);
            }

            if (await _repo.SaveAllAsync())
                return NoContent();

            throw new Exception("Error deleting the message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int id, int userId)
        {
            //Sprawdzenie czy użytkownik istnieje
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var message = await _repo.GetMessage(id);

            //Tylko użytkownik który jest odbiorcą wiadomości 
            //może zaznaczyć wiadomość jako przeczytaną
            if (message.RecipientId != userId)
                return Unauthorized();

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await _repo.SaveAllAsync();

            return NoContent();
        }
        
    }
}