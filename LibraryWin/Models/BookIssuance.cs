﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace LibraryWin.Models;

public partial class BookIssuance
{
    public int BookIssuanceId { get; set; }

    public int LibrarianId { get; set; }

    public int ReaderId { get; set; }

    public DateTime DateOfIssue { get; set; }

    public DateTime? DateOfReturn { get; set; }

    public DateTime DateOfPlannedReturn { get; set; }

    public string BookName { get; set; }

    public string BookIssueIsbn { get; set; }

    public int BookIssueCode { get; set; }

    public virtual Librarian Librarian { get; set; }

    public virtual Reader Reader { get; set; }
}