<div align="center">
  <img src="https://klustr.netlify.app/klustr.png" width="100px" style="border-radius:10px;" align="center" >
  <h1>Klustr API</h1>
  <div align="center" style="display:flex; gap:16px;">
    <img src="https://img.shields.io/github/languages/top/Klustr-RTC/klustr-api" alt="shields">
    <img src="https://img.shields.io/github/forks/Klustr-RTC/klustr-api" alt="shields">
    <img src="https://img.shields.io/github/stars/Klustr-RTC/klustr-api" alt="shields">
  </div>
</div>

#### [Live Site](https://klustr.netlify.app) | [Youtube Video](https://www.youtube.com/watch?v=KGh4CIZ1KHo)

## Project Description

The Klustr API is a backend service for the [Klustr chat application](https://github.com/Klustr-RTC/klustr-web), built using .NET Web API, SignalR for real-time communication, and PostgreSQL as the database. This API handles user authentication, room management, messaging, and audio/video chat functionalities.

## Prerequisites

Before you begin, ensure you have met the following requirements:

- .NET SDK installed.
- PostgreSQL database setup.
- Basic knowledge of .NET Web API, SignalR, and PostgreSQL.

## Technologies Used

- .NET Web API
- SignalR
- PostgreSQL
- JWT for authentication

## Installation

1. Clone the repository:

```sh
git clone https://github.com/Klustr-RTC/klustr-api.git
```
2. Navigate to the project directory:
```sh
cd klustr-api
```

5. Create a `.env` file in the root directory and add the environment variables as mentioned in the `.env.example` file in root directory.

6. Restore NuGet packages:

```sh
dotnet restore
```

7. Apply database migrations:
```sh
dotnet ef database update
```

8. Run the API:
```sh
dotnet run
```

9. Open [http://localhost:5147/swagger/index.html](http://localhost:5147/swagger/index.html) in your browser to view the api documenetation.

## Features

- **User Authentication**: Register and login users using JWT.
- **Room Management**: Create, update, delete, and manage rooms.
- **Messaging**: Real-time messaging using SignalR.
- **Audio/Video Chat**: WebRTC integration for audio and video communication.
- **Profile Management**: View and edit user profiles.
- **Link Generation**: Generate and manage shareable links for rooms.

## Contributors

<a href="https://github.com/Klustr-RTC/klustr-api/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Klustr-RTC/klustr-api" />
</a>

## Contact Information

For inquiries or support, please contact:

- [Email](mailto:nileshdarji28200@gmail.com)
