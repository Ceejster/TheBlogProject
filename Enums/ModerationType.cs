using System.ComponentModel;

namespace TheBlogProject.Enums
{
    public enum ModerationType
    {
        [Description("Hate speech")]
        HateSpeech,
        [Description("Offensive language")]
        Language,
        [Description("Drug references")]
        Drugs,
        [Description("Threatening speech")]
        Threatening,
        [Description("Sex talk")]
        Sexual,
        [Description("Political propaganda")]
        Political,
        [Description("Call for violence")]
        Violence
    }
}