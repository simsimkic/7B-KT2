namespace InitialProject.Model.DTO
{
    public class RatingsDTO
    {
        public string UserName { get; set; }
        public string AccommodationName { get; set; }
        public int Cleanliness { get; set; }
        public int Correctness { get; set; }
        public string Comment { get; set; }

        public RatingsDTO() { }

        public RatingsDTO(string userName, string accommodationName, int cleanliness, int correctness, string comment)
        {
            UserName = userName;
            AccommodationName = accommodationName;
            Cleanliness = cleanliness;
            Correctness = correctness;
            Comment = comment;
        }
    }   
}
