﻿

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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class NCRSNEntity : DbContext
{
    public NCRSNEntity()
        : base("name=NCRSNEntity")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public DbSet<User> Users { get; set; }

    public DbSet<Club> Clubs { get; set; }

    public DbSet<Event> Events { get; set; }

    public DbSet<Key> Keys { get; set; }

    public DbSet<ClubSubscribe> ClubSubscribes { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<EventLikeDislike> EventLikeDislikes { get; set; }

    public DbSet<EventRequest> EventRequests { get; set; }

    public DbSet<EventAttending> EventAttendings { get; set; }

    public DbSet<ClubModerator> ClubModerators { get; set; }

    public DbSet<Feedback> Feedbacks { get; set; }

}

}

