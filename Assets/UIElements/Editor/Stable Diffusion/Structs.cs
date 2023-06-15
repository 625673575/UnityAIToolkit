using System.Collections.Generic;

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
    [System.Serializable]
    public class UpscalingOption
    {
        public string name;
        public string model_name;
        public string model_path;
        public object model_url;
        public double scale;
    }

    [System.Serializable]
    public class UpscalingOptions
    {
        public List<UpscalingOption> array;
    }



}