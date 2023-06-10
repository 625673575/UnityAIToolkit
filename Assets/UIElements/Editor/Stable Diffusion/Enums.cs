namespace StableDiffusion
{

    public enum SamplerMethods
    {
        [StringValue("Euler a")]
        Euler_a,
        [StringValue("Euler")]
        Euler,
        [StringValue("LMS")]
        LMS,
        [StringValue("Heun")]
        Heun,
        [StringValue("DPM2")]
        DPM2,
        [StringValue("DPM2 a")]
        DPM2_a,
        [StringValue("DPM++ 2S a")]
        DPMpp_2S_a,
        [StringValue("DPM++ 2M")]
        DPMpp_2M,
        [StringValue("DPM++ SDE")]
        DPMpp_SDE,
        [StringValue("DPM fast")]
        DPM_fast,
        [StringValue("DPM adaptive")]
        DPM_adaptive,
        [StringValue("LMS Karras")]
        LMS_Karras,
        [StringValue("DPM2 Karras")]
        DPM2_Karras,
        [StringValue("DPM2 a Karras")]
        DPM2_a_Karras,
        [StringValue("DPM++ 2S a Karras")]
        DPMpp_2S_a_Karras,
        [StringValue("DPM++ 2M Karras")]
        DPMpp_2M_Karras,
        [StringValue("DPM++ SDE Karras")]
        DPMpp_SDE_Karras,
        [StringValue("DDIM")]
        DDIM,
        [StringValue("PLMS")]
        PLMS,
        [StringValue("UniPC")]
        UniPC
    }
    public enum UpscalerModels
    {
        [StringValue("None")]
        None,
        [StringValue("Latent")]
        Latent,
        [StringValue("Latent (antialiased)")]
        Latent_antialiased,
        [StringValue("Latent (bicubic)")]
        Latent_bicubic,
        [StringValue("Latent (bicubic antialiased)")]
        Latent_bicubic_antialiased,
        [StringValue("Latent (nearest)")]
        Latent_nearest,
        [StringValue("Latent (nearest-exact)")]
        Latent_nearest_exact,
        [StringValue("Lanczos")]
        Lanczos,
        [StringValue("Nearest")]
        Nearest,
        [StringValue("ESRGAN_4x")]
        ESRGAN_4x,
        [StringValue("LDSR")]
        LDSR,
        [StringValue("R-ESRGAN 4x+")]
        R_ESRGAN_4x,
        [StringValue("R-ESRGAN 4x+ Anime6B")]
        R_ESRGAN_4x_Anime6B,
        [StringValue("ScuNET GAN")]
        ScuNET_GAN,
        [StringValue("ScuNET PSNR")]
        ScuNET_PSNR,
        [StringValue("SwinIR 4x")]
        SwinIR_4x,
    }
    public enum BackgroundModels
    {
        [StringValue("None")]
        None,
        [StringValue("u2net")]
        u2net,
        [StringValue("u2netp")]
        u2netp,
        [StringValue("u2net_human_seg")]
        u2net_human_seg,
        [StringValue("u2net_cloth_seg")]
        u2net_cloth_seg,
        [StringValue("silueta")]
        silueta
    }
}
