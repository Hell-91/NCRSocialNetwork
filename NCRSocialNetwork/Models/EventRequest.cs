
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
    
public partial class EventRequest
{

    public EventRequest()
    {

        this.EventRequestFlag = "false";

        this.Event = new HashSet<Event>();

    }


    public int EventRequestId { get; set; }

    public string EventRequestEventTitle { get; set; }

    public string EventRequestEventDescription { get; set; }

    public int EventRequestClubId { get; set; }

    public int EventRequestUserId { get; set; }

    public string EventRequestFlag { get; set; }

    public System.DateTime EventRequestDate { get; set; }



    public virtual Club Club { get; set; }

    public virtual User User { get; set; }

    public virtual ICollection<Event> Event { get; set; }

}

}