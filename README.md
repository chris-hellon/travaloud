# Travaloud

Travaloud is a multitenant Blazor application that serves as a content management and booking system for multiple travel and hospitality tenants. Currently, it caters to three tenants:

1. Fuse Hostels & Travel
2. Vietnam Backpacker Hostels
3. Uncut Travel & Hospitality

## Features

- **Content Management**: Efficiently manage and organize travel destinations, accommodations, and booking options for multiple tenants.
- **Booking System**: Provide a seamless booking experience for users to reserve accommodations and services offered by the tenants.
- **Multitenancy**: Support for multiple tenants ensures each has its own segregated data and configuration, maintaining data integrity and security.
- **Tenant Customization**: Allow tenants to personalize their content and booking options according to their branding and service offerings.
- **User Authentication and Authorization**: Implement secure user authentication and authorization to ensure data privacy and access control.
- **Responsive UI**: Develop a responsive user interface using MudBlazor components, ensuring compatibility across various devices and screen sizes.
- **Rich Validation**: Utilize FluentValidation for comprehensive validation of user input, enforcing business rules and data integrity.
- **SEO Optimization**: Optimize the application for search engines to improve visibility and attract more users to the platform.
- **Performance Optimization**: Implement caching and other performance optimization techniques to ensure smooth and fast user experience.
- **Analytics Integration**: Integrate analytics tools to gather insights into user behavior and engagement, aiding in decision-making and feature enhancements.

## Tech Stack

- **Blazor .NET 8 Web App**: Building interactive web UIs using C# and HTML.
- **Clean Architecture**: Structuring the application into layers for better maintainability and testability.
- **CQRS with Mediator Pattern**: Separating concerns between commands and queries using mediator for communication.
- **Entity Framework Core with Ardalis Specification**: ORM for data access, providing rich querying capabilities with Ardalis Specification.
- **FluentValidation**: Library for validating input data and enforcing business rules.
- **MudBlazor**: UI component library for Blazor applications, providing pre-built components for building rich user interfaces.
- **Finbuckle Multitenant**: Framework for implementing multitenancy in ASP.NET Core applications.
