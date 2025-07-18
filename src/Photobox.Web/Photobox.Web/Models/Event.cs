﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Photobox.Web.Models;

public class Event
{
    /// <summary>
    /// Primary key of the PhotoBox.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    /// <summary>
    /// Name of the current Event
    /// </summary>
    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }

    /// <summary>
    /// The date, when the Event starts.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The date, when the Event ends.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Indicates, wheter the current event is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// The Gallery code used, to retreive the images
    /// </summary>
    [MaxLength(6)]
    public string EventCode { get; set; }

    /// <summary>
    /// Foreign key to the owning application user.
    /// </summary>
    public Guid UsedPhotoBoxId { get; set; }

    /// <summary>
    /// The Photobox that is used during the event.
    /// </summary>
    public PhotoBox UsedPhotoBox { get; set; }

    /// <summary>
    /// All images taken during the event.
    /// </summary>
    public ICollection<Image> Images { get; } = [];

    /// <summary>
    /// Foreign key to the owning application user.
    /// </summary>
    public Guid ApplicationUserId { get; set; }

    /// <summary>
    /// Navigation property to the owning application user.
    /// </summary>
    public required ApplicationUser ApplicationUser { get; set; }
}
