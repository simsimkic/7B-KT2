using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace InitialProject.Model.DTO
{
    public class GuideRatingDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Checkpoint { get; set; }
        public string Comment { get; set; }
        public double Rating { get; set; }
        public string Language { get; set; }
        public string Knowledge { get; set; }
        public string Interestingness { get; set; }
        public bool IsValid { get; set; }

        public List<string> Urls { get; set; }

        public GuideRatingDTO(int id, string username, string checkpoint, string comment, int knowledge, int language, int interestingness, bool isValid, List<string> urls)
        {
            Id = id;
            Username = username;
            Checkpoint = checkpoint;
            Comment = comment;
            Rating = getRating(knowledge, language, interestingness);
            IsValid = isValid;
            Knowledge = knowledge.ToString();
            Language = language.ToString();
            Interestingness = interestingness.ToString();
            Urls = urls;    
        }

        public double getRating(int knowledge, int language, int interestingness) 
        {
            return ((double)knowledge + (double)language + (double)interestingness) / 3;
   
        }
        public ImageSource IsValidImage
        {
            get
            {
                string imagePath = IsValid ? "/Resources/Images/green-check.png" : "/Resources/Images/red-x.png";
                return new BitmapImage(new Uri(imagePath, UriKind.Relative));
            }
        }
    }
}
