using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBlogProject.Enums
{
    public enum ModerationType
    {
        [Description("Hate speech")]
        [Display(Name = "Hate Speech")]
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