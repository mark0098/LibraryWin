﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace LibraryWin.Models;

public partial class PublishingHouse
{
    public int PublishingHouseId { get; set; }

    public string Name { get; set; }

    public string Coverage { get; set; }

    public byte[] Commercial { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}