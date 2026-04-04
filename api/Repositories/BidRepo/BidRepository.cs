using api.Data; // Uzywam Data/AppDbContext.cs aby uzyskac dostep do DbSet<Bid> i metod SaveChangesAsync(), ToListAsync(), FindAsync() itp.
using api.Models; // Uzywam Models/Bid.cs aby uzyskac dostep do klasy Bid i jej wlasciwosci, ktore sa potrzebne do implementacji metod repozytorium. czyli np Bid.Id, Bid.Amount, Bid.AuctionId, Bid.UserId itp.

using Microsoft.EntityFrameworkCore; 

namespace api.Repositories.BidRepo; // Okreslam namespace dla tej klasy, aby latwo bylo zorganizowac kod i uniknac konfliktow nazw z innymi klasami. Wskazuje ze ta klasa jest czescia repozytorium Bid, ktore jest odpowiedzialne za zarzadzanie danymi dotyczacymi ofert (bids) w aplikacji.

public class BidRepository : IBidRepository // Implementacja IBidRepository pokazuje to dla Repozytorium jakie musi zawierac metody, ktore zdefiniowalem w interfejsie/
{
    private readonly AppDbContext _context; // Prywatne pole _context typu AppDbContext, ktore jest uzywane do komunikacji z baza danych.

    public BidRepository(AppDbContext context) // Kontruktor klasy BidRepository, bierze jako parametr AppDbContext i przypisuje go do _context, co pozwala dla tej klasy na zarzadzanie danymi dotyczacymi bids w bazie danych.
    {
        _context = context; // Przypisuje wyzej odebrany context do _context
    }
    public async Task<IEnumerable<Bid>> GetAllAsync() // IEnumerable iteruje po kolekcji Bid i sprawdza czy sa jakies dane, jesli sa to zwraca te dane w formie listy, a jesli nie to zwraca pusta liste.
    {
        return await _context.Bids.ToListAsync();// Zapisuje to wszystko do listy i zwraca ta liste.
    }
    public async Task<Bid?> GetByIdAsync(long id) // Metoda GetByIdAsync przyjmuje jako parametr id typu int, ktore jest ide ntyfikatorem oferty (bid) w bazie danych. Metoda zwraca obiekt Bid lub null, jesli oferta o podanym id nie istnieje.
    {
        return await _context.Bids.FindAsync(id); // Uzywa metody FindAsync() ktora szuka w bazie danych oferty o podanym id i zwraca ja, jesli znajdzie, lub null, jesli nie znajdzie.
    }
    public async Task<Bid> AddBidAsync(Bid bid) // Przyjmuje jako parametr obiekt Bid, ktory zawiera dane nowej oferty. Dodaje ten obiekt do bazy danych i zapisuje zmiany.
    {
        await _context.Bids.AddAsync(bid); // Uzywa metody AddAsync() ktora dodaje nowy obiekt Bid do listy Bids w bazie danych.
        await _context.SaveChangesAsync(); // Zapisuje wszystkie zmiany w bazie danych.
        return bid;
    }
    public async Task<Bid> UpdateBidAsync(Bid bid) // Przyjmuje Bid ktory zawiera w sobie zaktualizowane dane oferty.
    {
        _context.Bids.Update(bid); // Aktualizuje te oferte w bazie danych
        await _context.SaveChangesAsync();// Zapisuje wszystkie zmiany w bazie danych.
        return bid;
    }
    public async Task DeleteBidAsync(long id) // Przyjmuje id 
    {
        var Bid = await _context.Bids.FindAsync(id); // Szuka w bazie danych id i przypisuje to do zmiennej Bid.
        if (Bid == null) // Sprawdza czy jest Bid jest null
        {
            throw new KeyNotFoundException($"Bid with id {id} not found"); // Rzuca wyjatek KeyNotFoundException gdy Bid jest null
        }
        else
        {
            _context.Bids.Remove(Bid); // Usuwa ten Bid
            await _context.SaveChangesAsync(); // Zapisuje zmiany w bazie danych
        }
    }
}