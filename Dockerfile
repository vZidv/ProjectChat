FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . ./

RUN dotnet restore "./ChatServer/ChatServer/ChatServer.csproj"

RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet publish "./ChatServer/ChatServer/ChatServer.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY --from=build /app/out .
COPY --from=build /app/ChatServer/ChatServer/. .

# Install ef tools in final image
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# ... previous content ...

# Create startup script with longer initial wait
RUN echo '#!/bin/bash\n\
echo "Waiting for SQL Server to start..."\n\
sleep 30\n\
echo "Attempting database migration..."\n\
until dotnet ef database update --verbose; do\n\
    echo "Migration failed, retrying in 10s..."\n\
    sleep 10\n\
done\n\
echo "Starting Chat Server..."\n\
exec dotnet ChatServer.dll' > /app/entrypoint.sh \
    && chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]