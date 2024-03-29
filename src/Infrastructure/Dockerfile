FROM tensorflow/tensorflow:latest

WORKDIR /app

RUN mkdir -p /universal_sentence_encoder \
    && pip install tensorflow-hub \
    && apt-get update && apt-get install -y --no-install-recommends \
        ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY Scripts/app.py Scripts/universal_sentence_encoder.py /app/

EXPOSE 63778

CMD ["python", "app.py"]
