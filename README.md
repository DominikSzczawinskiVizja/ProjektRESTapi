# DOKUMENTACJA TECHNICZNA
# System Aukcji Internetowych - Alledrogo

## 1. Instrukcja Uruchomienia Systemu

### Wymagania Wstępne
- **.NET 10 SDK** - [Pobierz tutaj](https://dotnet.microsoft.com/download/dotnet/10.0)
- **SQL Server LocalDB** - instalowany automatycznie z Visual Studio lub [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- **Visual Studio 2026** lub dowolny edytor kodu (VSCode, Rider) (nie zostały sprawdzone)
- **Git** - do klonowania repozytorium

### Konfiguracja Początkowa

Przed uruchomieniem aplikacji **wymagane** jest ustawienie User Secrets dla JWT Key.

#### Krok 1: Ustaw JWT Secret Key

Otwórz **PowerShell** lub **Command Prompt** w folderze `api`:

```powershell
cd api

# Ustaw Jwt:Key (minimum 32 znaki)
dotnet user-secrets set "Jwt:Key" "SuperTajnyKlucz123456789012345678901234567890"

# Sprawdź czy się ustawił
dotnet user-secrets list
```

**Oczekiwany wynik:**
```
Jwt:Key = SuperTajnyKlucz123456789012345678901234567890
```

#### Krok 2: Zastosuj Migracje Bazy Danych

```powershell
# W folderze `api`
dotnet ef database update
```

Baza danych `AuctionDb` zostanie automatycznie utworzona w SQL Server LocalDB.

---

### Opcja 1: Uruchomienie w Visual Studio (Rekomendowane)

1. **Otwórz solution** (`ProjektRESTapi.sln`) w Visual Studio 2022
2. **Ustaw projekt startowy** → Kliknij prawym przyciskiem na `api` → Set as Startup Project
3. **Naciśnij F5** lub przycisk "Run"

**Wynik:**
- Backend (API) uruchomi się na: `https://localhost:7240`
- Swagger dostępny na: `https://localhost:7240/swagger`

Jeśli chcesz uruchomić również Frontend:
- W Solution Explorer zaznacz **oba projekty** (`api` i `razor`)
- Kliknij prawym przyciskiem → Set as Startup Projects → Multiple startup projects
- Ustaw oba na "Start"
- Frontend uruchomi się automatycznie

---

### Opcja 2: Uruchomienie z Terminala (PowerShell / Command Prompt)

#### Backend API

```powershell
# Przejdź do folderu api
cd api

# Uruchom aplikację
dotnet run

# Wynik w terminalu:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: https://localhost:7240
```

Swagger dostępny na: `https://localhost:7240/swagger`

#### Frontend Razor Pages

W **nowym terminalu**:

```powershell
# Przejdź do folderu razor
cd razor

# Uruchom aplikację
dotnet run

# Wynik:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: https://localhost:5286
```

Frontend dostępny na: `https://localhost:5286`

---

### Opcja 3: Uruchomienie w Docker (Zaawansowane)

#### Wymagania Docker
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Docker musi być uruchomiony

#### Backend API w Docker

```powershell
# Będąc w głównym folderze projektu
cd api

# Stwórz obraz Docker
docker build -t alledrogo-api .

# Uruchom kontener z ustawionym JWT Key (zmień klucz)
docker run -e Jwt__Key="ThisIsAVeryLongSecretKeyWith32CharactersMinimumForJwt1234" `
           -p 7240:7240 `
           -v logs:/app/logs `
           alledrogo-api

# API dostępny na: https://localhost:7240
```

#### Docker Compose (Całe środowisko)

Jeśli chcesz uruchomić API + Frontend + baza danych razem, utwórz plik `docker-compose.yml`:

```yaml
version: '3.8'

services:
  api:
    build:
      context: ./api
      dockerfile: Dockerfile
    environment:
      Jwt__Key: "ThisIsAVeryLongSecretKeyWith32CharactersMinimumForJwt1234"
      ConnectionStrings__DefaultConnection: "Server=mssql;User Id=sa;Password=YourStrong!Pass123;Database=AuctionDb;"
    ports:
      - "7240:7240"
    depends_on:
      - mssql
    volumes:
      - ./logs:/app/logs

  razor:
    build:
      context: ./razor
      dockerfile: Dockerfile
    environment:
      Api__BaseUrl: "https://api:7240/"
    ports:
      - "5286:5286"
    depends_on:
      - api

  mssql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "YourStrong!Pass123"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    volumes:
      - mssql_data:/var/opt/mssql/data

volumes:
  mssql_data:
```

Uruchomienie:

```powershell
docker-compose up -d
```

---

### Sprawdzenie czy wszystko działa

#### API

```bash
# Sprawdź czy API jest dostępny
curl https://localhost:7240/api/auctions

# Lub otwórz w przeglądarce
https://localhost:7240/swagger
```

#### Frontend

Otwórz w przeglądarce:
```
https://localhost:5286
```

---

### Troubleshooting

#### Problem: "Invalid JWT configuration"
**Przyczyna:** Brakuje `Jwt:Key` w User Secrets  
**Rozwiązanie:**
```powershell
cd api
dotnet user-secrets set "Jwt:Key" "ThisIsAVeryLongSecretKeyWith32CharactersMinimumForJwt1234"
```

#### Problem: "Cannot open database 'AuctionDb'"
**Przyczyna:** Migracje nie zostały zastosowane  
**Rozwiązanie:**
```powershell
cd api
dotnet ef database update
```

#### Problem: "Cannot connect to SQL Server"
**Przyczyna:** SQL Server LocalDB nie jest zainstalowany  
**Rozwiązanie:** 
- Zainstaluj Visual Studio z opcją SQL Server LocalDB lub
- Zainstaluj [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)

#### Problem: Port 7240 jest już zajęty
**Rozwiązanie:** Zmień port w `api/Properties/launchSettings.json`:
```json
"https": {
  "applicationUrl": "https://localhost:7241;http://localhost:5168"
}
```

---

## 2. Opis Architektury Systemu
System zostal zbudowany jako dwie oddzielne aplikacje: backend w postaci REST API oraz frontend w postaci aplikacji Razor Pages

### 2.1 Warstwy systemu
Backend (REST API) jest podzielony na następujące warstwy:
- Controllers - warstwa prezentacji, przyjmuje zadania HTTP i zwraca odpowiedzi JSON
- Services - logika biznesowa, walidacja zasad domenowych
- Repositories - dostęp do bazy danych przez Entity Framework Core
- Models - klasy mapowane na tabele SQL
- DTOs - obiekty transferu danych (oddzielenie modelu od API)
- Middleware - globalna obsługa wyjątków
- Authorization - niestandardowe handlery polityk autoryzacji

Frontend (Razor Pages) to aplikacja Razor Pages komunikująca się z backendem wyłącznie przez HTTP REST API.

### 2.2 Przepływ danych
- Użytkownik wykonuje akcje w przeglądarce (Razor Pages)
- Frontend wysyła zadanie HTTP do REST API z tokenem JWT w nagłówku Authorization
- ExceptionMiddleware przechwytuje ewentualne błędy
- Controller odbiera zadanie, waliduje dane wejściowe i przekazuje do Service
- Service wykonuje logikę biznesowa i wywołuje Repository
- Repository wykonuje zapytanie do SQL Server przez Entity Framework Core
- Odpowiedz wędruje z powrotem przez te same warstwy jako obiekt DTO

### 2.3 Technologie

Warstwa	Technologia	Opis
Backend API	ASP.NET Core 10 Web API	Framework REST API
ORM / baza danych	Entity Framework Core + SQL Server	Dostęp do danych relacyjnych
Autentykacja	Microsoft.AspNetCore.Authentication.JwtBearer	Tokeny dostępu JWT
Hashowanie hasel	ASP.NET Core Identity PasswordHasher	Bezpieczne przechowywanie haseł
Logowanie	Serilog (konsola + pliki)	Logi w folderze logs/
Dokumentacja API	Swagger / OpenAPI	Dostępny w trybie Development
Frontend	Razor Pages (.NET 10)	Aplikacja webowa MPA
Style CSS	Bootstrap 5	Responsywny interfejs

---

## 3. Diagram ERD i klas
Baza danych składa się z trzech tabel: Users, Auctions i Bids
klas:
┌──────────────────────────────┐
│         User                 │
├──────────────────────────────┤
│ - Id: long                   │
│ - FirstName: string          │
│ - LastName: string           │
│ - Email: string              │
│ - PasswordHash: string       │
├──────────────────────────────┤
│ + GetFullName(): string      │
│ + ValidateEmail(): bool      │
└──────────────────────────────┘
           ▲
           │ Owns (1)
           │
┌──────────────────────────────┐
│       Auction                │
├──────────────────────────────┤
│ - Id: long                   │
│ - Name: string               │
│ - CurrentPrice: decimal      │
│ - EndAt: DateTime            │
├──────────────────────────────┤
│ + PlaceBid(): void           │
│ + IsActive(): bool           │
└──────────────────────────────┘
           ▲
           │ Has Many (1..*) 
           │
┌──────────────────────────────┐
│        Bid                   │
├──────────────────────────────┤
│ - Id: long                   │
│ - Amount: decimal            │
│ - CreatedAt: DateTime        │
├──────────────────────────────┤
│ + Validate(): bool           │
└──────────────────────────────┘
ERD:
┌─────────────┐
│    Users    │
├─────────────┤
│ Id (PK)     │
│ FirstName   │
│ Email       │───┐
│ Password    │   │
└─────────────┘   │
      ▲           │
      │ (1)       │ (*)
      │           │
      └───────────┘
                  │
            ┌─────────────┐
            │  Auctions   │
            ├─────────────┤
            │ Id (PK)     │
            │ Name        │
            │ OwnerId(FK)─┘
            └─────────────┘
                  │
                  │ (1)
                  │ (*)
                  │
            ┌─────────────┐
            │    Bids     │
            ├─────────────┤
            │ Id (PK)     │
            │ Amount      │
            │ AuctionId(FK)
            └─────────────┘
### 3.1 Tabela Users
Kolumna	Typ	Opis
Id	BIGINT (PK)	Klucz główny, autoinkrementacja
FirstName	NVARCHAR (wymagane)	Imię użytkownika
MiddleName	NVARCHAR (opcjonalne)	Drugie imię
LastName	NVARCHAR (wymagane)	Nazwisko użytkownika
Email	NVARCHAR (wymagane)	Adres e-mail (unikalny)
PasswordHash	NVARCHAR (wymagane)	Zahashowane hasło
Address	NVARCHAR (wymagane)	Adres zamieszkania
Role	NVARCHAR 	Rola: User lub Admin
CreatedAt	DATETIME	Data rejestracji (UTC)

### 3.2 Tabela Auctions
Kolumna	Typ	Opis
Id	BIGINT (PK)	Klucz główny
Name	NVARCHAR (wymagane)	Nazwa przedmiotu
Description	NVARCHAR (wymagane)	Opis aukcji
Category	NVARCHAR (wymagane)	Kategoria przedmiotu
Price	DECIMAL(18,2) (wymagane)	Cena wywoławcza
CurrentPrice	DECIMAL(18,2)	Aktualna najwyższa oferta
StartAt	DATETIME	Data rozpoczęcia aukcji
EndAt	DATETIME (wymagane)	Data zakończenia aukcji
CreatedAt	DATETIME	Data dodania rekordu (UTC)
OwnerId	BIGINT (FK - Users.Id)	Właściciel aukcji

### 3.3 Tabela Bids
Kolumna	Typ	Opis
Id	BIGINT (PK)	Klucz główny
Amount	DECIMAL(18,2) (wymagane)	Kwota oferty
AuctionId	BIGINT (FK - Auctions.Id)	Aukcja, do której należy oferta
UserId	BIGINT (FK - Users.Id)	Użytkownik składający ofertę
CreatedAt	DATETIME	Data złożenia oferty (UTC)

### 3.4 Relacje miedzy tabelami
- User (1) - (*) Auction: jeden użytkownik może wystawiać wiele aukcji (pole OwnerId)
- User (1) - (*) Bid: jeden użytkownik może składać wiele ofert (pole UserId)
- Auction (1) - (*) Bid: jedna aukcja może mieć wiele ofert (pole AuctionId)

---

## 4. Opis Endpointow API
Wszystkie endpointy zwracają dane w formacie JSON. Dokumentacja interaktywna jest dostępna pod adresem /swagger po uruchomieniu API w trybie Development.

### 4.1 Autentykacja (zadne nie wymaga tokenu)
Metoda	Endpoint	Opis	Body (JSON)
POST	/api/auth/register	Rejestracja nowego uzytkownika, zwraca token JWT	{ firstName, lastName, email, password, address }
POST	/api/auth/login	Logowanie, zwraca token JWT	{ email, password }

### 4.2 Uzytkownicy - /api/users
Metoda	Endpoint	Autoryzacja	Opis
GET	/api/users/{id}	Bearer token	Pobiera dane użytkownika po ID
POST	/api/users	Bearer token	Dodaje nowego użytkownika
PUT	/api/users/{id}	Bearer token (wlasciciel)	Aktualizuje dane użytkownika
DELETE	/api/users/{id}	Bearer token (wlasciciel)	Usuwa konto użytkownika
GET	/api/users	Rola Admin	Lista wszystkich użytkowników

### 4.3 Aukcje - /api/auctions
Metoda	Endpoint	Autoryzacja	Opis
GET	/api/auctions	Brak	Lista wszystkich aukcji
GET	/api/auctions/{id}	Brak	Szczegóły aukcji po ID
POST	/api/auctions	Bearer token	Dodanie nowej aukcji
PUT	/api/auctions/{id}	Polityka UserRoleStatus	Edycja aukcji (tylko właściciel)
DELETE	/api/auctions/{id}	Polityka UserRoleStatus	Usuniecie aukcji (tylko właściciel)
GET	/api/auctions/{id}/winner	Polityka UserRoleStatus	Zwycięzca aukcji (po jej zakończeniu)

### 4.4 Oferty - /api/bids
Metoda	Endpoint	Autoryzacja	Opis
POST	/api/bids/{auctionId}	Bearer token	Złożenie oferty na aukcje
GET	/api/bids/{id}	Rola Admin	Pobranie oferty po ID (tylko admin)

### 4.5 Kody HTTP zwracane przez API
Kod	Znaczenie	Kiedy wystepuje
200 OK	Sukces	GET, PUT - poprawne pobranie lub aktualizacja
201 Created	Zasób utworzony	POST - nowy użytkownik lub aukcja
204 No Content	Brak treści	DELETE - zasób usunięty
400 Bad Request	Błędne dane	Walidacja danych wejściowych, np. za niska oferta
401 Unauthorized	Brak autoryzacji	Brak lub niepoprawny token JWT
403 Forbidden	Brak uprawnień	Użytkownik próbuje edytować cudza aukcje
404 Not Found	Nie znaleziono	Aukcja lub użytkownik nie istnieje
500 Internal Server Error	Błąd serwera	Nieobsłużony wyjątek

---

## 5. Autoryzacja JWT
System korzysta z tokenów JWT (JSON Web Token) do weryfikacji tożsamości użytkowników.

### 5.1 Jak działa autoryzacja
- Użytkownik loguje się przez POST /api/auth/login i otrzymuje token JWT
- Token zawiera Claims: UserId, Email, Role (User lub Admin)
- Token jest ważny 60 minut (konfigurowalne w appsettings.json)
- Każde chronione zadanie musi zawierać nagłówek: Authorization: Bearer {token}
- Middleware JWT weryfikuje podpis, wystawce, odbiorcę i ważność tokenu

### 5.2 Konfiguracja JWT
```
{
  "Jwt": {
    "Issuer": "ProjektRESTapi",
    "Audience": "ProjektRESTapi",
    "ExpiresMinutes": 60
  }
}
```

---

## 6. Walidacja Danych i Obsługa Błędów

### 6.1 Walidacja danych wejściowych
- [Required] - pole wymagane
- [EmailAddress] - poprawny format adresu e-mail
- [Range(1, double.MaxValue)] - cena aukcji musi być większa od 1
- [RegularExpression] - imiona mogą zawierać tylko litery

### 6.2 Globalna obsluga wyjatkow
Typ wyjatku	Kod HTTP	Przyklad
KeyNotFoundException	404 Not Found	Aukcja o podanym ID nie istnieje
InvalidOperationException	400 Bad Request	Oferta niższa od aktualnej ceny; aukcja zakończona
UnauthorizedAccessException	401 Unauthorized	Błędne hasło przy logowaniu
ForbiddenException	403 Forbidden	Próba edycji cudzej aukcji
Exception (pozostale)	500 Internal Server Error	Nieoczekiwany błąd serwera

### 6.3 Logika biznesowa ofert
- Sprawdza, czy czas aukcji nie minął (EndAt < DateTime.UtcNow) - jeśli tak, rzuca InvalidOperationException
- Sprawdza, czy kwota oferty jest wyższa od CurrentPrice - jeśli nie, rzuca InvalidOperationException
- Po pomyślnej walidacji aktualizuje CurrentPrice w Auctions i zapisuje nowa ofertę do Bids

---

## 7. Logowanie Operacji (Serilog)
- Konsola - na bieżąco podczas działania aplikacji
- Pliki - w folderze logs/ z rotacja dzienna (api-log-YYYYMMDD.txt)

Logowane zdarzenia:
- Informacja o uruchomieniu aplikacji
- Ostrzeżenia przy wyjątkach obsługiwanych przez middleware (404, 400, 401, 403)
- Błędy wewnętrzne serwera (500)
- Fatal - gdy aplikacja nie może się uruchomić

---

## 8. Interfejs Uzytkownika (Razor Pages)
Frontend to aplikacja Razor Pages (.NET 10) komunikujaca sie z backendem wylacznie przez HTTP REST API.

Strona	Plik .cshtml	Opis
Strona glowna	Index.cshtml	Widok powitalny systemu
Lista aukcji	Aukcje.cshtml	Przegladanie dostępnych aukcji
Szczegoly aukcji	Aukcja.cshtml	Szczegóły i formularz składania oferty
Dodaj aukcje	add_auction.cshtml	Formularz wystawiania przedmiotu
Edytuj aukcje	edytuj_aukcje.cshtml	Edycja własnej aukcji
Profil uzytkownika	Profil.cshtml	Wyświetlenie danych konta
Rejestracja	register.cshtml	Formularz rejestracji nowego konta
Wylogowanie	Logout.cshtml	Wylogowanie użytkownika
Panel admina	Admin.cshtml	Zarzadzanie użytkownikami (tylko Admin)

---

## 9. Struktura Projektu

### 9.1 Backend (REST API)
api/
  Controllers/     - AuctionsController, UsersController, BidsController, AuthController
  Services/        - AuctionService, UserService, BidService, AuthService, TokenService
  Repositories/    - AuctionRepository, UserRepository, BidRepository
  Models/          - Auction.cs, User.cs, Bid.cs
  DTOs/            - AuctionDto/, UserDto/, BidDto/, AuthDto/
  Data/            - AppDbContext.cs
  Middleware/      - ExceptionMiddleware.cs
  Authorization/   - AuctionStatusHandler.cs, StatusRequirement.cs
  Migrations/      - InitialCreate
  Program.cs       - konfiguracja DI, middleware, JWT, Swagger

### 9.2 Frontend (Razor Pages)
razor/
  Pages/           - Index, Aukcje, Aukcja, add_auction, edytuj_aukcje, Profil, register...
  Pages/Shared/    - _Layout.cshtml (Bootstrap 5)
  wwwroot/         - zasoby statyczne (CSS, JS, Bootstrap, jQuery)

# Wykonali Jakub Piekart i Dominik Szczawiński
