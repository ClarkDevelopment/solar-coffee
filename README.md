# solar-coffee
Simple .Net Core Web API managing fictional coffee shop "Solar Coffee"'s customers and inventory.
- This project pairs with repository "solarcoffe.frontend" utilizing a Vuejs front end UI.

## Endpoints:

### Customers

- Retrieve all Customers

  - Protocol: GET

  - Route: /api/customer

  - Return Data: 

```{
  Id, 
  FirstName, 
  LastName, 
  PrimaryAddress: {
    Id,
    CreatedOn,
    UpdatedOn,
    AddressLine1,
    AddressLine2,
    City,
    State,
    Postalcode,
    Country
  },
  CreatedOn,
  UpdatedOn
}
```

- Retrieve Specific Customer

  - Protocol: GET
  
  - Route: /api/customer/{id}
  
  - Return Data:
  
```{
  Id, 
  FirstName, 
  LastName, 
  PrimaryAddress: {
    Id,
    CreatedOn,
    UpdatedOn,
    AddressLine1,
    AddressLine2,
    City,
    State,
    Postalcode,
    Country
  },
  CreatedOn,
  UpdatedOn
}
```

- Create New Customer
  - Protocol: POST
  - Route - /api/customer
  - Return Data:
  
 ```
 {
   IsSuccess,
   Message,
   Time,
   Data: {
     {
      Id, 
      FirstName, 
      LastName, 
      PrimaryAddress: {
        Id,
        CreatedOn,
        UpdatedOn,
        AddressLine1,
        AddressLine2,
        City,
        State,
        Postalcode,
        Country
      },
      CreatedOn,
      UpdatedOn
    }
   }
 }
 ```



