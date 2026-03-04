# PrzetrwajPL
PrzetrwajPL is a specialized web application designed to enhance community safety by allowing users to report and track local dangers. This platform bridges the gap between citizens and real-time situational awareness.

## 📑 Project documentation & business logic
Before coding, we focused heavily on the business and planning.
* **Project specification & dictionary**: To ensure clear communication, we developed a professional dictionary defining terms like "User", "Visitor", "Danger", "Region".
* **Database modeling**: We designed an entity relation diagram.
* **UML architecture**: We created UML class diagrams to define the system's logic and service interactions.
* **Backlog management**: We prepared a backlog with tasks, allowing for efficient work distribution.

## 🔑 Key features
* Viewing, adding and confirming posts.
* Personalized home page for ease of staying on top of current events.
* Moderator and Administrator privileges.
* Personal data protection. Your personal data is limited to minimal and is not accessible to anyone but you on the page.

## 🏗️ Architecture
The application is built using a modern decoupled architecture to ensure scalability and performance. Containerized environment ensures a consistent setup for all developers.

### 🖥️ Frontend (Blazor Web App)
The frontend is built with ASP.NET Core Blazor, utilizing Interactive Server and WebAssembly (WASM) render modes for a seamless user experience.
* Interactive danger reporting: Users can create detailed posts about local threats, including title, description category and images.
* Location awareness: Integrated Region Picker components allow users to tag posts and profiles to specific geographic areas.
* Profile management: A dedicated user settings page allows for real-time updates to personal information, region preferences, and security credentials.

### ⚙️ Backend (REST API)
The backend serves as the secure engine of the application, managing data persistence and complex business logic. <br>
**Role-Based Access Control** (RBAC): Fine-grained security policies distinguish between standard Users, Moderators, and Administrators<br>
Database: PostgreSQL for robust, relational data storage.
* Multi-provider login: Supports standard email/password credentials and integrated Google OAuth for convenience.
* JWT: Employs JSON Web Tokens for API authorization and secure HTTP-only connection.
* Verification: Features mandatory email verification to ensure a verified and accountable user base.

## 📝 Lessons learned
As this was our first experience working in a shared professional repository. We successfully learned how to:
* **Manage workflows**: Use Pull Requests, create feature branches and resolve merge conflicts.
* **Delegate tasks**: Assigning a task to a person prevents multiple people doing the same work.
* **Menage expectations**: Our original plans included making an entire Moderator dashboard for moderating the service and map diplay for danger visualisation.
* **Team balance**: We have learned that building the frontend from zero requires much more work, than an equally split team between front and back end can provide.

## 🌐 Check it out:
Thanks to our university we can run it in the Azure cloud.
* ### [Przetrwaj](https://przetrwaj-front.grayflower-7f624026.polandcentral.azurecontainerapps.io/)

## 👥 The team
This project was a collaborative effort by:
* Patryk Niesporek: [fafik](github.com/u/fafik777)
* Krzysztof Ludwiczak: [KrzysztofLudwiczak](https://github.com/KrzysztofLudwiczak)
* Dominika Zakrzewska: [Bananarisss](https://github.com/Bananarisss)
* Karol Kubica: [Karol337](https://github.com/Karol337)
* Adam Chorzelski: 
* Grzegorz Kozłowski: 
