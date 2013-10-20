
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace NCRSocialNetwork.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Club
{

    public Club()
    {

        this.ClubThumbnailPath = "\\Content\\Images\\ClubThumbnail.png";

        this.ClubImagePath = "\\Content\\Images\\ClubImage.png";

        this.Events = new HashSet<Event>();

        this.EventRequests = new HashSet<EventRequest>();

        this.ClubSubscribe = new HashSet<ClubSubscribe>();

        this.ClubModerators = new HashSet<ClubModerator>();

    }


    public int ClubId { get; set; }

    public string ClubName { get; set; }

    public string ClubDescription { get; set; }

    public string ClubThumbnailPath { get; set; }

    public string ClubImagePath { get; set; }



    public virtual ICollection<Event> Events { get; set; }

    public virtual Key Key { get; set; }

    public virtual ICollection<EventRequest> EventRequests { get; set; }

    public virtual ICollection<ClubSubscribe> ClubSubscribe { get; set; }

    public virtual ICollection<ClubModerator> ClubModerators { get; set; }

}

}