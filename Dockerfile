FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY origami_api/origami_api.csproj origami_api/
RUN dotnet restore origami_api/origami_api.csproj

COPY origami_api/ origami_api/
RUN dotnet publish origami_api/origami_api.csproj -c Release -o /out/origami_api /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:10000 \
    ASPNETCORE_ENVIRONMENT=Production \
    Recognition__PythonExecutable=/usr/bin/python3 \
    Recognition__BridgeScriptPath=../ai/api_predict.py \
    Recognition__ModelPath=../ai/origami_model.h5

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        python3 \
        python3-pip \
        libglib2.0-0 \
        libgl1 \
        libsm6 \
        libxext6 \
        libxrender1 \
    && rm -rf /var/lib/apt/lists/*

COPY requirements.txt ./requirements.txt
RUN python3 -m pip install --no-cache-dir --break-system-packages -r requirements.txt

COPY --from=build /out/origami_api ./origami_api
COPY ai ./ai
COPY origami_sample_images ./origami_sample_images

WORKDIR /app/origami_api
EXPOSE 10000

ENTRYPOINT ["dotnet", "origami_api.dll"]
