using Microsoft.EntityFrameworkCore;

using Entities;

using RepositoryContracts;

using System.Linq.Expressions;

using Bogus;

using Person = Entities.Person;

namespace Repositories;

public class PersonsRepository : IPersonsRepository
{
    private readonly ApplicationDbContext _db;

    public PersonsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Person> AddPerson(Person person)
    {
        _db.Persons.Add(person);
        await _db.SaveChangesAsync();

        return person;
    }

    public async Task<bool> DeletePersonByPersonID(Guid personID)
    {
        _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonID == personID));
        int rowsDeleted = await _db.SaveChangesAsync();

        return rowsDeleted > 0;
    }

    public async Task<List<Person>> GetAllPersons()
    {
        return await _db.Persons.Include("Country").ToListAsync();
    }

    public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
    {
        return await _db.Persons.Include("Country")
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Person?> GetPersonByPersonID(Guid personID)
    {
        return await _db.Persons.Include("Country")
            .FirstOrDefaultAsync(temp => temp.PersonID == personID);
    }

    public async Task<Person> UpdatePerson(Person person)
    {
        Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);

        if (matchingPerson == null)
            return person;

        matchingPerson.PersonName = person.PersonName;
        matchingPerson.Email = person.Email;
        matchingPerson.DateOfBirth = person.DateOfBirth;
        matchingPerson.Gender = person.Gender;
        matchingPerson.CountryID = person.CountryID;
        matchingPerson.Address = person.Address;
        matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;
        await _db.SaveChangesAsync();
        return matchingPerson;
    }

    public Task Delete50Persons()
    {
        _db.Persons.RemoveRange(_db.Persons.Take(50));
        return _db.SaveChangesAsync();
    }

    public Task Generate20Persons()
    {
        var countryIds = _db.Countries.Select(temp => temp.CountryID).ToList();
        
        var persons = new Faker<Person>()
            .RuleFor(temp => temp.PersonID, temp => Guid.NewGuid())
            .RuleFor(temp => temp.PersonName, temp => temp.Name.FullName())
            .RuleFor(temp => temp.Email, temp => temp.Internet.Email())
            .RuleFor(temp => temp.DateOfBirth, temp => temp.Date.Past(100))
            .RuleFor(temp => temp.CountryID, temp => temp.PickRandom(countryIds))
            .RuleFor(temp => temp.Address, temp => temp.Address.FullAddress())
            .RuleFor(temp => temp.ReceiveNewsLetters, temp => temp.Random.Bool())
            .RuleFor(temp => temp.TIN, temp => temp.Finance.Iban())
            .Generate(20);
        
        _db.Persons.AddRange(persons);
        return _db.SaveChangesAsync();
        
    }
}