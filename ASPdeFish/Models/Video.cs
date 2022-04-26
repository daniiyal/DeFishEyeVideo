namespace ASPdeFish.Models
{
    public class Video
    {
        public string ThumbName { get; set; }
        public string VideoName { get; set; }

        public string Factor { get; set; }

        public Video(string thumbPath, string videoPath)
        {
            ThumbName = thumbPath;
            VideoName = videoPath;
        }
    }

}
