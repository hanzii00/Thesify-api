#  Thesify API

An AI-powered REST API that generates capstone project ideas for students using **Groq AI (Llama 3.3)** and **ASP.NET Core**.

---

##  Tech Stack

- **ASP.NET Core 9** — Web API framework
- **Groq API** — Free AI inference (Llama 3.3 70B)
- **Swagger UI** — Built-in API documentation and testing
- **C# 13** — Language

---

##  Project Structure

```
CapstoneGenerator.API/
├── Controllers/
│   └── CapstoneController.cs     # POST /api/capstone/generate
├── Services/
│   └── GroqService.cs            # Calls Groq AI API
├── Models/
│   ├── RequestDto.cs             # Input model
│   └── ResponseDto.cs            # Output model
├── Helpers/
│   └── PromptBuilder.cs          # Builds the AI prompt
├── appsettings.json              # Safe placeholder config (committed)
├── appsettings.Development.json  # Real API key (NOT committed)
├── Program.cs                    # App bootstrap + DI
└── CapstoneGenerator.API.csproj  # Project dependencies
```

---

##  Setup & Installation

### 1. Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [VSCode](https://code.visualstudio.com/) with C# Dev Kit extension
- Free Groq API key from [console.groq.com](https://console.groq.com)

### 2. Clone the repository
```bash
git clone https://github.com/your-username/CapstoneGenerator.API.git
cd CapstoneGenerator.API
```

### 3. Add your Groq API key
Create `appsettings.Development.json` in the project root:
```json
{
  "Groq": {
    "ApiKey": "gsk_your_groq_api_key_here"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

>  This file is in `.gitignore` — your key will never be committed.

### 4. Restore dependencies
```bash
dotnet restore
```

### 5. Run the API
```bash
dotnet run
```

Swagger UI will be available at: `http://localhost:5147`

---

## 🔌 API Reference

### `POST /api/capstone/generate`

Generates a capstone project idea based on student input.

**Request Body:**
```json
{
  "course": "Computer Science",
  "difficulty": "Intermediate",
  "interests": ["AI", "healthcare"],
  "timeframe": "6 months",
  "budget": "$300",
  "notes": "Prefer web-based projects"
}
```

**Fields:**
| Field | Type | Required | Description |
|---|---|---|---|
| `course` | string | ✅ | Student's course or major |
| `difficulty` | string | ✅ | Beginner / Intermediate / Advanced |
| `interests` | string[] | ✅ | At least one interest |
| `timeframe` | string | ❌ | Project duration |
| `budget` | string | ❌ | Available budget |
| `notes` | string | ❌ | Additional preferences |

**Response `200 OK`:**
```json
{
  "title": "AI-Powered Medical Diagnosis Web Application",
  "description": "A web-based medical diagnosis system using ML algorithms...",
  "features": [
    "Symptom Checker",
    "Disease Prediction",
    "Medical Recommendation System"
  ],
  "tech_stack": [
    "Python",
    "TensorFlow",
    "Flask",
    "HTML/CSS/JavaScript"
  ],
  "methodology": "Agile"
}
```

**Error Responses:**
| Code | Reason |
|---|---|
| `400` | Missing required fields |
| `502` | Groq API unreachable or key invalid |
| `500` | Unexpected server error |

---

##  Testing with Postman

1. Set method to **POST**
2. URL: `http://localhost:5147/api/capstone/generate`
3. Go to **Body** → **raw** → **JSON**
4. Paste the request body above
5. Hit **Send**

---

## Security Notes

- Never commit `appsettings.Development.json`
- Add the following to `.gitignore`:
```
appsettings.Development.json
bin/
obj/
```

---

## Architecture

```
POST /api/capstone/generate
        ↓
CapstoneController (input validation)
        ↓
PromptBuilder (formats AI prompt)
        ↓
GroqService (calls Llama 3.3 via Groq API)
        ↓
JSON parsed into CapstoneResponse
        ↓
200 OK returned to client
```

---

## License

This project is currently unlicensed. All rights reserved by the author.
