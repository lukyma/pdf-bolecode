#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM surnet/alpine-wkhtmltopdf:3.16.0-0.12.6-small as wkhtmltopdf
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
RUN apk update && apk add libgdiplus --no-cache --repository https://dl-cdn.alpinelinux.org/alpine/edge/testing --allow-untrusted && \  
		apk add --no-cache   \ 
		libstdc++   \
		libx11   \
		libxrender   \
		libxext   \
		libssl1.1   \
		ca-certificates   \
		fontconfig   \
		freetype   \
		ttf-dejavu   \
		ttf-droid   \
		ttf-freefont   \
		ttf-liberation && \
		apk add --no-cache --virtual .build-deps   \
		msttcorefonts-installer && \
		update-ms-fonts && fc-cache -f && rm -rf /tmp/* && apk del .build-deps # buildkit

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ./ ./
RUN dotnet restore "CreatePdf/CreatePdf.csproj"
RUN dotnet build "CreatePdf/CreatePdf.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "CreatePdf/CreatePdf.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=wkhtmltopdf /bin/wkhtmltopdf /app/wwwroot/rotativa
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "CreatePdf.dll"]