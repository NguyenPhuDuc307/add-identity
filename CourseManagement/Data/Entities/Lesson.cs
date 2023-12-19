using System.Net.Mime;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Data.Entities;

public class Lesson
{
    public int Id { get; set; }
    [MaxLength(200)]
    [Required]
    public string? Title { get; set; }
    [MaxLength(200)]
    public string? ImagePath { get; set; }
    [Required]
    public string? Introduction { get; set; }
    public string? Content { get; set; }
    [DataType(DataType.Date)]
    public DateTime DateCreated { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }
}