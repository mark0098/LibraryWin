﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace LibraryWin.Models;

public partial class HallEmployee
{
    public int HallEmployeeId { get; set; }

    public int HallId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string SurName { get; set; }

    public string Post { get; set; }

    public decimal Salary { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string WorkSchelude { get; set; }

    public TimeSpan WorkingHours { get; set; }

    public virtual Hall Hall { get; set; }
}