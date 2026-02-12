﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("Documents")]
public class Document
{
    [Key]
    public int DocumentId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? DocumentType { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    public long? FileSize { get; set; }

    [Required]
    [ForeignKey("Uploader")]
    public int UploadedBy { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public virtual User Uploader { get; set; } = null!;
}

