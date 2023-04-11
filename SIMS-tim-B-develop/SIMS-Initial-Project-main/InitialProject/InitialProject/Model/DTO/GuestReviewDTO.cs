namespace InitialProject.Model.DTO
{
    public class GuestReviewDTO
    {
        public int ReservationId { get; set; }
        public string UserName { get; set; }
        public string AccommodationName { get; set; }

        public GuestReviewDTO() { }

        public GuestReviewDTO(int reservationId, string userName, string accommodationName)
        {
            ReservationId = reservationId;
            UserName = userName;
            AccommodationName = accommodationName;
        }
    }
}
