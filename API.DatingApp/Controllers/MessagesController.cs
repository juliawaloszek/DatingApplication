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

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            //Sprawdzenie czy użytkownik istnieje
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
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

            //przekształcenie Message => messageForCreationDto
            var messageToReturn = _mapper.Map<MessageForCreationDto>(message);

            //CreatedAtRoute(Name of root/nazwa ścieżki, objekt do przekazania)
            if(await _repo.SaveAllAsync())
                return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);

            throw new Exception("Creating the messsafe failed on save");
        }
        
    }
}