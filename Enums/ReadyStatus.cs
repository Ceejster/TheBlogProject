using System.ComponentModel.DataAnnotations;

namespace TheBlogProject.Enums
{
    public enum ReadyStatus
    {
        Incomplete,
        [Display(Name = "Production Ready")]
        ProductionReady,
        [Display(Name = "Preview Ready")]
        PreviewReady
    }
}
