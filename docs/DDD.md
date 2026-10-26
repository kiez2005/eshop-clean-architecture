# Project Domain: Business Rules & Logic

This document describes the **business domain** of the project, including the entities, their rules (invariants), and the key processes that drive the system. Use this as a guide to understand *what* the system does, rather than *how* the code is structured.

## 1. Domain Aggregates

The system is built around four main aggregates: **Categories**, **Products**, **Baskets**, and **Orders**.

### 1.1. Product
Represents an item available for sale.
*   **Attributes**: Name, Description, Price, ImagePath, CategoryId.
*   **Rules (Invariants)**:
    *   **Price**: Must be non-negative.
    *   **Name**: Cannot be empty or whitespace.
    *   **Category**: Every product must belong to a valid Category (`CategoryId != Guid.Empty`).
*   **Methods**:
    *   `UpdateDetails`: Modifies descriptive info (Name, Description, ImagePath, CategoryId).
    *   `UpdatePrice`: Changes price, ensuring it never goes below zero.

### 1.2. Basket (Shopping Cart)
Represents a user's temporary collection of items before purchase.
*   **Attributes**: User Identity, List of Items (`BasketProductItem`), computed `TotalPrice`.
*   **Rules**:
    *   **Items**: Contains products with a specific quantity and snapshot price/name.
    *   **Merging**: If a product is added that already exists in the basket, the quantity is summed rather than creating a duplicate entry.
    *   **Quantity**: Must always be greater than zero.
*   **Methods**:
    *   `AddBasketProductItem`: Adds an item or increments quantity with price snapshot.
    *   `RemoveBasketProductItem`: Removes a specific product line entirely.
    *   `Clear`: Empties the basket.

### 1.3. Order
Represents a finalized purchase agreement.
*   **Attributes**: User ID, User Information (Snapshot), Order Items, Status, computed `TotalPrice`.
*   **Status Lifecycle**:
    1.  `Pending`: Default state when order is created.
    2.  `Completed`: Transitioned via `MarkAsCompleted()` (disallowed if already `Cancelled`).
    3.  `Cancelled`: Transitioned via `Cancel()` (disallowed if already `Completed`).
*   **Rules**:
    *   **User Information**: Must be captured at the time of order (Name, Phone, Address) as an immutable value object snapshot.
    *   **Non-Empty Items**: An order must contain at least one `OrderItem` with positive quantity and non-negative price.
    *   **Immutability**: Once created, the Order Items collection is read-only.
*   **Events**:
    *   `OrderCreatedDomainEvent`: Raised immediately when an order is created and persisted via the transactional **Outbox Pattern**.

## 2. Shared Business Concepts (Value Objects)

These are reusable concepts that carry specific validation rules used across the system:

*   **Phone**:
    *   Must be exactly **10 digits** and non-empty.
*   **Address**:
    *   Requires both **Street** and **City** to be non-empty.
*   **UserInformation**:
    *   Composite value object comprising Name, `Phone`, and `Address`.
    *   Used to snapshot customer delivery details on an Order, independent of future account profile updates.

## 3. Key Business Processes

### 3.1. Checkout Process
The transition from **Basket** to **Order**:

1.  **Trigger**: User initiates checkout from their Basket.
2.  **Input**: Delivery information (`Name`, `Phone`, `Address`) provided by the user.
3.  **Process**:
    *   The system validates the User's Basket exists and is not empty.
    *   The `CheckoutService` domain service extracts line items from the Basket along with the provided `UserInformation`.
    *   It creates a new `Order` aggregate and raises `OrderCreatedDomainEvent`.
    *   The Basket is **Cleared** immediately within the same transaction.
4.  **Result**: A new Order ID is returned, and the `OrderCreatedDomainEvent` is stored in `OutboxMessages` for reliable background processing (sending order confirmation email via Hangfire).
