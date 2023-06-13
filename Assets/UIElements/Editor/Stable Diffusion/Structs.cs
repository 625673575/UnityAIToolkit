using System.Collections.Generic;
using UnityEngine;
namespace StableDiffusion
{
    public struct Progress
    {
        public float progress;
        public float eta_relative;
        public struct ProgressState
        {
            public bool skipped;
            public bool interrupted;
            public string job;
            public int job_count;
            public int job_timestamp;
            public int job_no;
            public int sampling_step;
            public int sampling_steps;
        }
        public ProgressState state;
        public string current_image;
        public string textinfo;
    }
}