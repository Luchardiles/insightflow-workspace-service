# InsightFlow - Workspace Service

## 📋 Información del Proyecto

**Asignatura:** Taller de Ingeniería de Software  
**Universidad:** Universidad Católica del Norte  
**Servicio:** Workspace Service  

---

## 🏗️ Arquitectura

Este proyecto implementa una **Arquitectura de Microservicios** donde cada servicio es independiente y se comunica mediante APIs RESTful.

### Patrón de Diseño
- **Repository Pattern:** Para abstraer la lógica de acceso a datos (en memoria)
- **Minimal API:** Utilizando las características de .NET 8 para crear APIs ligeras
- **Dependency Injection:** Para gestionar las dependencias del servicio

---

## 🚀 Descripción del Servicio

El **Workspace Service** gestiona los espacios de trabajo colaborativos de InsightFlow. Un espacio de trabajo es un contenedor para documentos y tareas donde múltiples usuarios pueden colaborar.

### Funcionalidades Implementadas

1. **Crear Espacio de Trabajo** (`POST /workspaces`)
   - Asigna un UUID v4 único
   - Valida nombres únicos
   - Asigna automáticamente al creador como "Propietario"

2. **Listar Espacios por Usuario** (`GET /workspaces?userId={guid}`)
   - Retorna todos los espacios donde el usuario es miembro
   - Incluye el rol del usuario en cada espacio

3. **Obtener Espacio por ID** (`GET /workspaces/{id}`)
   - Retorna información detallada del espacio
   - Lista completa de miembros con sus roles

4. **Actualizar Espacio** (`PATCH /workspaces/{id}`)
   - Solo propietarios pueden editar
   - Valida nombres únicos al cambiar

5. **Eliminar Espacio** (`DELETE /workspaces/{id}`)
   - Implementa SOFT DELETE
   - Solo propietarios pueden eliminar

---

## 📦 Tecnologías Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core Minimal APIs** - Para crear endpoints REST
- **Swagger/OpenAPI** - Documentación automática de la API
- **Docker** - Contenedorización del servicio
- **GitHub Actions** - Pipeline CI/CD
- **Docker Hub** - Registro de imágenes Docker
- **Render** - Plataforma de despliegue en la nube

---

## 🔧 Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Git](https://git-scm.com/)

---

## 💻 Instalación y Ejecución Local

### Opción 1: Ejecutar con .NET CLI

```bash
# 1. Clonar el repositorio
git clone https://github.com/tu-usuario/insightflow-workspace.git
cd insightflow-workspace

# 2. Restaurar dependencias
dotnet restore

# 3. Ejecutar el proyecto
dotnet run

# 4. Acceder a Swagger UI
# Abre tu navegador en: http://localhost:5265/swagger
```

### Opción 2: Ejecutar con Docker

```bash
# 1. Construir la imagen Docker
docker build -t insightflow-workspace .

# 2. Ejecutar el contenedor
docker run -p 8080:8080 insightflow-workspace

# 3. Acceder a Swagger UI
# Abre tu navegador en: http://localhost:8080/swagger
```

---

## 📡 Endpoints Disponibles

### Base URL (Local)
```
http://localhost:5265
```

### Base URL (Producción)
```
https://tu-servicio.onrender.com
```

### Endpoints

| Método | Endpoint | Descripción | Requiere Auth |
|--------|----------|-------------|---------------|
| POST | `/workspaces` | Crear nuevo espacio de trabajo | ✅ |
| GET | `/workspaces?userId={guid}` | Listar espacios de un usuario | ✅ |
| GET | `/workspaces/{id}` | Obtener espacio por ID | ✅ |
| PATCH | `/workspaces/{id}` | Actualizar espacio | ✅ (Propietario) |
| DELETE | `/workspaces/{id}?userId={guid}` | Eliminar espacio (soft delete) | ✅ (Propietario) |

---

## 📝 Ejemplos de Uso

### Crear un Espacio de Trabajo

```bash
POST /workspaces
Content-Type: application/json

{
  "name": "Proyecto Final",
  "description": "Espacio para el desarrollo del proyecto final",
  "theme": "Educación",
  "iconUrl": "https://via.placeholder.com/150",
  "userId": "550e8400-e29b-41d4-a716-446655440001",
  "userName": "Juan Pérez"
}
```

**Respuesta:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "Proyecto Final",
  "description": "Espacio para el desarrollo del proyecto final",
  "theme": "Educación",
  "iconUrl": "https://via.placeholder.com/150",
  "ownerId": "550e8400-e29b-41d4-a716-446655440001",
  "createdAt": "2025-12-14T10:30:00Z"
}
```

### Listar Espacios de un Usuario

```bash
GET /workspaces?userId=550e8400-e29b-41d4-a716-446655440001
```

**Respuesta:**
```json
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "name": "Proyecto Final",
    "iconUrl": "https://via.placeholder.com/150",
    "role": "Propietario",
    "theme": "Educación",
    "createdAt": "2025-12-14T10:30:00Z"
  }
]
```

---

## 🔄 Pipeline CI/CD

El proyecto implementa un pipeline completo de CI/CD usando GitHub Actions:

### Flujo Automático

1. **Trigger:** Push o merge a la rama `main`
2. **Build:** Construcción de la imagen Docker
3. **Push:** Publicación en Docker Hub
4. **Deploy:** Despliegue automático en Render

### Configuración de Secrets en GitHub

Necesitas configurar los siguientes secrets en tu repositorio:

```
DOCKER_USERNAME         # Tu usuario de Docker Hub
DOCKER_PASSWORD         # Tu token de acceso de Docker Hub
RENDER_DEPLOY_HOOK_URL  # URL del webhook de Render
```

### Pasos para Configurar Secrets

1. Ve a tu repositorio en GitHub
2. Settings → Secrets and variables → Actions
3. Clic en "New repository secret"
4. Agrega cada secret con su valor correspondiente

---

## 🐳 Docker Hub

### Publicar Manualmente

```bash
# 1. Login en Docker Hub
docker login

# 2. Construir la imagen
docker build -t tu-usuario/insightflow-workspace:latest .

# 3. Publicar la imagen
docker push tu-usuario/insightflow-workspace:latest
```

---

## ☁️ Despliegue en Render

### Pasos para Configurar Render

1. **Crear cuenta en Render:** https://render.com
2. **New Web Service** → "Deploy an existing image from a registry"
3. **Configuración:**
   - Image URL: `tu-usuario/insightflow-workspace:latest`
   - Port: `8080`
4. **Obtener Deploy Hook:**
   - Settings → Deploy Hook → Copy URL
   - Agregar como secret `RENDER_DEPLOY_HOOK_URL` en GitHub

---

## 📚 Datos de Prueba (Seeder)

El servicio incluye datos de ejemplo que se cargan automáticamente al iniciar:

### Usuarios de Ejemplo
- Usuario 1: `550e8400-e29b-41d4-a716-446655440001` (Juan Pérez)
- Usuario 2: `550e8400-e29b-41d4-a716-446655440002` (María González)

### Espacios de Trabajo de Ejemplo
1. **Proyecto Universidad** (Propietario: Juan Pérez)
2. **Ideas Personales** (Propietario: Juan Pérez)
3. **Desarrollo Web** (Propietario: María González)

---

## 🧪 Testing

### Probar con cURL

```bash
# Listar espacios de un usuario
curl "http://localhost:5000/workspaces?userId=550e8400-e29b-41d4-a716-446655440001"

# Obtener un espacio específico
curl "http://localhost:5000/workspaces/{workspace-id}?userId=550e8400-e29b-41d4-a716-446655440001"
```

### Probar con Postman

Importa la colección de Postman incluida en el repositorio: `workspace-service.postman_collection.json`


---