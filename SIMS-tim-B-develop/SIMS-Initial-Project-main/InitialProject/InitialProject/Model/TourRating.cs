using InitialProject.Serializer;
using System;
using System.Collections.ObjectModel;

namespace InitialProject.Model
{
    public class TourRating : ISerializable
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int UserId { get; set; }
        public int GuideKnowledge { get; set; }
        public int GuideLanguage { get; set; }
        public int Interestingness { get; set; }
        public String Comment { get; set; }

        public ObservableCollection<int> ImageIds { get; set; }

        public bool isValid { get; set; } 

        public TourRating() { ImageIds = new ObservableCollection<int>(); }

        public TourRating(int tourId, int guideKnowledge, int guideLanguage, int interestingness, string comment, ObservableCollection<int> imageIds, int userId)
        {
            TourId = tourId;
            GuideKnowledge = guideKnowledge;
            GuideLanguage = guideLanguage;
            Interestingness = interestingness;
            Comment = comment;
            ImageIds = imageIds;
            UserId = userId;
            isValid = true;
        }

        public string[] ToCSV()
        {
            string[] csvValues =
                {   Id.ToString(),
                    TourId.ToString(),
                    UserId.ToString(),
                    GuideKnowledge.ToString(),
                    GuideLanguage.ToString(),
                    Interestingness.ToString(),
                    Comment,
                    string.Join(",", ImageIds),
                    isValid.ToString() };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            TourId = Convert.ToInt32(values[1]);
            UserId = Convert.ToInt32(values[2]);
            GuideKnowledge = Convert.ToInt32(values[3]);
            GuideLanguage = Convert.ToInt32(values[4]);
            Interestingness = Convert.ToInt32(values[5]);
            Comment = values[6];
            foreach (string id in values[7].Split(','))
            {
                ImageIds.Add(int.Parse(id));
            }
            isValid = bool.Parse(values[8]);
        }
    }
}
