# Deadlock API

A minimal Web API for managing **Heroes**, **Items**, and **Builds**.

- **Base URL (local dev):** https://localhost:
- **Docs/Testing:** Swagger UI at /swagger
- **Content Type:** application/json

## Heroes

### GET /heroes

- Fetch all heroes.
- Response: list of { id, name }.

### POST /heroes

- Create a new hero.
- Request:
  ```json
  { "name": "Avernus" }
  ```
- Responses:
  - 201 Created with { id, name }
  - 400 Bad Request if invalid payload

---

## Items

### GET /items

- Fetch all items.
- Response: list of items.

### GET /items/{id}

- Fetch a single item.
- Responses:
  - 200 OK with { id, name, price, color }
  - 404 Not Found

### POST /items

- Create a new item.
- Notes: color must be one of Orange, Purple, Green (case-insensitive).
- Request:
  ```json
  { "name": "Storm Pike", "price": 3200, "color": "Orange" }
  ```
- Responses:
  - 201 Created with item details
  - 400 Bad Request if color invalid

### PUT /items/{id}

- Update an item (partial update allowed; fields optional).
- Request:
  ```json
  { "name": "Storm Pike Mk II", "price": 3450, "color": "Purple" }
  ```
- Responses:
  - 204 No Content on success
  - 400 Bad Request if invalid color
  - 404 Not Found

### DELETE /items/{id}

- Delete an item.
- Responses:
  - 204 No Content
  - 404 Not Found

---

## Builds

A **Build** belongs to a **Hero** and can include multiple **Items**.

### POST /builds

- Create a build.
- Validates heroId exists; if itemIds provided, each must exist.
- Request:
  ```json
  {
    "name": "Burst Crit",
    "desc": "High burst, glass cannon.",
    "heroId": 2,
    "itemIds": [10, 11]
  }
  ```
- Responses:
  - 201 Created with { id, name, desc, heroId }
  - 400 Bad Request if hero or any item not found (e.g., { "error": "Hero 2 not found." })

### GET /builds/{id}

- Fetch a build including its items.
- Responses:
  - 200 OK:
    ```json
    {
      "id": 5,
      "name": "Burst Crit",
      "desc": "High burst, glass cannon.",
      "heroId": 2,
      "items": [
        { "id": 10, "name": "Storm Pike", "price": 3200, "color": "Orange" }
      ]
    }
    ```
  - 404 Not Found

---

## Status Codes

- 200 OK – Success
- 201 Created – Resource created
- 204 No Content – Successful update/delete
- 400 Bad Request – Validation error
- 404 Not Found – Resource missing

---

## Example cURL

```bash
# Create hero
curl -X POST https://localhost:5001/heroes \
  -H "Content-Type: application/json" \
  -d '{ "name": "Avernus" }'

# Create item
curl -X POST https://localhost:5001/items \
  -H "Content-Type: application/json" \
  -d '{ "name": "Storm Pike", "price": 3200, "color": "Orange" }'

# Create build with items
curl -X POST https://localhost:5001/builds \
  -H "Content-Type: application/json" \
  -d '{ "name": "Burst Crit", "desc": "High burst", "heroId": 1, "itemIds": [10] }'

# Get build
curl https://localhost:5001/builds/5
```
