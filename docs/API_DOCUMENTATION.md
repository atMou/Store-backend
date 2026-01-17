# ?? API Documentation

## Base URL
```
https://api.yourstore.com
```

## Authentication

All authenticated endpoints require a JWT token in the Authorization header:

```http
Authorization: Bearer <your_jwt_token>
```

## Modules & Endpoints

### ?? Identity & Authentication

#### Register User
```http
POST /auth/register
Content-Type: multipart/form-data

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "SecurePass123!",
  "age": 25,
  "gender": "Male",
  "avatar": <file>
}
```

**Response:**
```json
{
  "userId": "guid",
  "email": "john@example.com",
  "cartId": "guid"
}
```

#### Login
```http
POST /auth/login

{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response:**
```json
{
  "user": {
    "id": "guid",
    "email": "john@example.com",
    "firstName": "John",
    "lastName": "Doe"
  },
  "accessToken": "eyJhbGc...",
  "refreshToken": "Set in HttpOnly cookie"
}
```

#### Email Verification
```http
POST /auth/verify

{
  "email": "john@example.com",
  "code": "123456",
  "token": "verification_token",
  "rememberMe": true
}
```

---

### ??? Products

#### Get Products (Paginated)
```http
GET /products?pageNumber=1&pageSize=10&brandCode=1&colorCode=2

Query Parameters:
- pageNumber: int (default: 1)
- pageSize: int (default: 10, max: 50)
- brandCode: int (optional)
- colorCode: int (optional)
- sizeCode: int (optional)
- minPrice: decimal (optional)
- maxPrice: decimal (optional)
- search: string (optional)
```

**Response:**
```json
{
  "items": [
    {
      "id": "guid",
      "slug": "nike-air-max",
      "brand": "Nike",
      "colors": ["Red", "Blue"],
      "sizes": ["S", "M", "L"],
      "price": 129.99,
      "images": [
        {
          "url": "https://...",
          "isMain": true
        }
      ],
      "colorVariants": [
        {
          "id": "guid",
          "color": "Red",
          "sizeVariants": [
            {
              "id": "guid",
              "size": "M",
              "sku": "NIKE-AM-RED-M",
              "price": 129.99,
              "stockLevel": "InStock"
            }
          ]
        }
      ]
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 100,
  "totalPages": 10
}
```

#### Get Product by Slug
```http
GET /products/{slug}?include=ColorVariants,Images,Reviews
```

---

### ?? Shopping Cart

#### Get User's Cart
```http
GET /baskets/{userId}/user
Authorization: Bearer <token>
```

**Response:**
```json
{
  "cartId": "guid",
  "userId": "guid",
  "totalSub": 99.99,
  "discount": 10.00,
  "totalAfterDiscounted": 89.99,
  "tax": 9.00,
  "shipmentCost": 4.50,  // Free if total >= $29.50
  "total": 103.49,
  "deliveryAddress": {
    "street": "123 Main St",
    "city": "New York",
    "postalCode": 10001,
    "houseNumber": 123
  },
  "lineItems": [
    {
      "productId": "guid",
      "colorVariantId": "guid",
      "sku": "NIKE-AM-RED-M",
      "size": "M",
      "quantity": 2,
      "unitPrice": 49.99,
      "lineTotal": 99.98,
      "imageUrl": "https://..."
    }
  ],
  "coupons": [
    {
      "code": "SAVE10",
      "discount": 10.00
    }
  ]
}
```

#### Add Items to Cart
```http
POST /baskets/add-multiple-line-items

{
  "cartId": "guid",
  "items": [
    {
      "productId": "guid",
      "colorVariantId": "guid",
      "sizeVariantId": "guid",
      "quantity": 2
    }
  ]
}
```

#### Update Line Item Quantity
```http
POST /baskets/update-line-item
Authorization: Bearer <token>

{
  "cartId": "guid",
  "colorVariantId": "guid",
  "sizeVariantId": "guid",
  "quantity": 3
}
```

#### Delete Line Item
```http
DELETE /baskets/delete-line-item

{
  "cartId": "guid",
  "variantId": "guid"
}
```

#### Apply Coupon
```http
POST /baskets/add-coupon

{
  "cartId": "guid",
  "couponId": "guid"
}
```

#### Update Delivery Address
```http
POST /baskets/change-delivery-address

{
  "cartId": "guid",
  "street": "456 Oak Ave",
  "city": "Los Angeles",
  "postalCode": 90001,
  "houseNumber": 456,
  "extraDetails": "Apt 2B"
}
```

---

### ?? Orders

#### Checkout Cart
```http
POST /baskets/checkout
Authorization: Bearer <token>

{
  "cartId": "guid"
}
```

**Response:**
```json
{
  "orderId": "guid",
  "status": "Pending",
  "message": "Order created successfully. Proceed to payment."
}
```

#### Get Order by ID
```http
GET /orders/{orderId}
Authorization: Bearer <token>
```

**Response:**
```json
{
  "orderId": "guid",
  "userId": "guid",
  "email": "john@example.com",
  "status": "Paid",
  "subtotal": 99.99,
  "tax": 10.00,
  "discount": 5.00,
  "shipmentCost": 4.50,
  "total": 109.49,
  "trackingCode": "TRK-20250106-1234",
  "shippingAddress": {
    "street": "123 Main St",
    "city": "New York",
    "postalCode": 10001
  },
  "orderItems": [
    {
      "orderItemId": "guid",
      "productId": "guid",
      "sku": "NIKE-AM-RED-M",
      "size": "M",
      "quantity": 2,
      "unitPrice": 49.99,
      "lineTotal": 99.98,
      "imageUrl": "https://..."
    }
  ],
  "paymentId": "guid",
  "shipmentId": "guid"
}
```

#### Get User's Orders
```http
GET /orders/user/{userId}?pageNumber=1&pageSize=10
Authorization: Bearer <token>
```

---

### ?? Payments

#### Create Payment Intent
```http
POST /payments/create-payment-intent

{
  "orderId": "guid",
  "userId": "guid"
}
```

**Response:**
```json
{
  "clientSecret": "pi_xxx_secret_xxx",
  "publishableKey": "pk_test_xxx"
}
```

#### Confirm Payment
```http
POST /payments/confirm

{
  "paymentIntentId": "pi_xxx",
  "orderId": "guid"
}
```

#### Stripe Webhook (Internal)
```http
POST /payments/webhook
Stripe-Signature: xxx

{
  "type": "payment_intent.succeeded",
  "data": {
    "object": {
      "id": "pi_xxx",
      "metadata": {
        "order_id": "guid"
      }
    }
  }
}
```

---

### ?? Shipments

#### Get Shipment by Order
```http
GET /shipments/order/{orderId}
Authorization: Bearer <token>
```

**Response:**
```json
{
  "shipmentId": "guid",
  "orderId": "guid",
  "trackingCode": "TRK-20250106-1234",
  "status": "Shipped",
  "shippingAddress": {
    "street": "123 Main St",
    "city": "New York",
    "postalCode": 10001
  },
  "shippedAt": "2025-01-06T10:30:00Z",
  "deliveredAt": null
}
```

#### Update Shipment Status (Admin)
```http
PUT /shipments/{id}/status
Authorization: Bearer <admin_token>

{
  "status": "OutForDelivery"
}
```

**Possible statuses:**
- `Pending`
- `Shipped`
- `InTransit`
- `OutForDelivery`
- `Delivered`
- `OnHold`
- `Cancelled`

---

### ?? Inventory

#### Get Inventory by Variant
```http
GET /inventory/variant/{variantId}
```

**Response:**
```json
{
  "inventoryId": "guid",
  "colorVariantId": "guid",
  "stock": 50,
  "stockLevel": "InStock",
  "warehouseLocation": "A-12-3"
}
```

#### Update Inventory (Admin)
```http
PUT /inventory/{id}
Authorization: Bearer <admin_token>

{
  "stock": 100,
  "warehouseLocation": "B-14-5"
}
```

---

### ??? Coupons

#### Get Available Coupons
```http
GET /coupons/available
```

#### Create Coupon (Admin)
```http
POST /coupons
Authorization: Bearer <admin_token>

{
  "code": "SAVE20",
  "discountPercentage": 20,
  "discountAmount": 0,
  "expiresAt": "2025-12-31T23:59:59Z",
  "maxUses": 100
}
```

---

## Real-Time SignalR Hub

### NotificationHub

#### Connect
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://api.yourstore.com/hubs/notifications", {
        accessTokenFactory: () => userToken
    })
    .withAutomaticReconnect()
    .build();

await connection.start();
```

#### Subscribe to Order Updates
```javascript
await connection.invoke("SubscribeToOrder", orderId);
```

#### Listen for Notifications
```javascript
// Shipment status updates
connection.on("ReceiveShipmentStatusUpdate", (notification) => {
    console.log(`Shipment ${notification.status}: ${notification.message}`);
    console.log(`Tracking: ${notification.trackingCode}`);
});

// Order status updates
connection.on("ReceiveOrderStatusUpdate", (notification) => {
    console.log(`Order ${notification.status}: ${notification.message}`);
});

// Payment updates
connection.on("ReceivePaymentUpdate", (notification) => {
    console.log(`Payment ${notification.status}: ${notification.message}`);
});

// Stock alerts
connection.on("ReceiveStockAlert", (notification) => {
    console.log(`${notification.productName} is back in stock!`);
});
```

---

## Error Responses

All errors follow this format:

```json
{
  "type": "ValidationError",
  "title": "Validation Failed",
  "status": 400,
  "detail": "Email format is invalid",
  "instance": "/api/auth/register",
  "errors": [
    {
      "code": "INVALID_EMAIL",
      "message": "Email format is invalid"
    }
  ]
}
```

### Error Types

| Status | Type | Description |
|--------|------|-------------|
| 400 | `ValidationError` | Invalid input data |
| 401 | `UnauthorizedError` | Missing or invalid token |
| 403 | `ForbiddenError` | Insufficient permissions |
| 404 | `NotFoundError` | Resource not found |
| 409 | `ConflictError` | Resource conflict (e.g., email exists) |
| 422 | `UnprocessableEntityError` | Business rule violation |
| 500 | `InternalServerError` | Server error |

---

## Rate Limiting

- **Anonymous users**: 100 requests per minute
- **Authenticated users**: 500 requests per minute
- **Admin users**: 1000 requests per minute

Rate limit headers:
```http
X-RateLimit-Limit: 500
X-RateLimit-Remaining: 499
X-RateLimit-Reset: 1609459200
```

---

## Pagination

All list endpoints support pagination:

**Request:**
```http
GET /products?pageNumber=1&pageSize=20
```

**Response:**
```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

## Filtering & Sorting

### Products
```http
GET /products?
    brandCode=1&
    colorCode=2&
    minPrice=50&
    maxPrice=200&
    sortBy=price&
    sortOrder=asc
```

### Orders
```http
GET /orders/user/{userId}?
    status=Delivered&
    fromDate=2025-01-01&
    toDate=2025-12-31&
    sortBy=createdAt&
    sortOrder=desc
```

---

## Webhooks (For External Integration)

### Order Status Changed
```json
{
  "event": "order.status_changed",
  "orderId": "guid",
  "status": "Shipped",
  "timestamp": "2025-01-06T10:30:00Z"
}
```

### Shipment Delivered
```json
{
  "event": "shipment.delivered",
  "orderId": "guid",
  "shipmentId": "guid",
  "trackingCode": "TRK-20250106-1234",
  "deliveredAt": "2025-01-10T14:30:00Z"
}
```

---

## SDK Examples

### C# Client
```csharp
var client = new StoreApiClient("https://api.yourstore.com");
await client.AuthenticateAsync("email", "password");

var products = await client.Products.GetAllAsync(
    pageNumber: 1,
    pageSize: 10,
    brandCode: BrandCode.Nike
);

await client.Cart.AddItemAsync(cartId, new AddLineItemRequest
{
    ProductId = productId,
    ColorVariantId = colorVariantId,
    SizeVariantId = sizeVariantId,
    Quantity = 2
});
```

### JavaScript Client
```javascript
const client = new StoreApiClient('https://api.yourstore.com');
await client.auth.login('email', 'password');

const products = await client.products.getAll({
    pageNumber: 1,
    pageSize: 10,
    brandCode: 1
});

await client.cart.addItem(cartId, {
    productId,
    colorVariantId,
    sizeVariantId,
    quantity: 2
});
```

---

## Testing

### Postman Collection
Download the Postman collection: [Store-Backend.postman_collection.json](./Store-Backend.postman_collection.json)

### Swagger/OpenAPI
Interactive API documentation: `https://api.yourstore.com/swagger`

---

## Support

- **Documentation**: https://docs.yourstore.com
- **Issues**: https://github.com/atMou/Store-Backend/issues
- **Email**: support@yourstore.com
