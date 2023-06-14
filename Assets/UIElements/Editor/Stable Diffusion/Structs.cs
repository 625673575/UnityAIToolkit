namespace StableDiffusion
{
    [System.Serializable]
    public class Progress
    {
        public double progress;
        public double eta_relative;
        public ProgressState state;
        public object current_image;
        public object textinfo;
    }

    [System.Serializable]
    public class ProgressState
    {
        public bool skipped;
        public bool interrupted;
        public string job;
        public int job_count;
        public string job_timestamp;
        public int job_no;
        public int sampling_step;
        public int sampling_steps;
    }


}