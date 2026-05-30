# OrderSystem
event-driven e-commerce order system across three independent services

The system consists of three independently deployable .NET services that communicate
asynchronously through RabbitMQ — no direct HTTP calls between services.
Services
• Order Service — ASP.NET Core Web API. Accepts POST /orders, saves to db,
publishes OrderCreatedEvent to RabbitMQ.
•Inventory Service — .NET BackgroundService. Consumes orders.created queue,
checks/reserves stock, publishes OrderConfirmedEvent.
• Notification Service — .NET BackgroundService. Consumes orders.confirmed queue,
sends confirmation (logged to console or email).
# Message Flow
• Client → POST /orders → Order Service
• Order Service → publishes to RabbitMQ Exchange (topic: orders.*)
• Exchange → routes orders.created → Inventory Queue
• Inventory Service → processes, publishes orders.confirmed
• Exchange → routes orders.confirmed → Notification Queue
• Notification Service → sends confirmation

