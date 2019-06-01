using System;

namespace API.DatingApp.Dtos
{
    public class MessageToReturnDto
    {
        public int Id { get; set; }

        //Ważne jest aby umieścieć propertki dotyczące danej klasy poniżej ID SenderId => SenderKnownAs, SenderPhotoUrl
        //Dzięki temu AutoMappert pobierze je uatomatycznie
        public int SenderId { get; set; }
        public string SenderKnownAs { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int RecipientId { get; set; }
        public string RecipientKnownAs { get; set; }
        public string RecipientPhotoUrl { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }
}