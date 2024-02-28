FROM tensorflow/tensorflow:latest

WORKDIR /app

RUN apt-get update && apt-get install -y \
    wget \
    unzip

COPY Scripts/download.py /app/download.py

RUN mkdir -p /universal_sentence_encoder
RUN pip install tensorflow-hub
RUN python download.py

COPY Scripts/app.py /app/app.py
COPY Scripts/universal_sentence_encoder.py /app/universal_sentence_encoder.py

EXPOSE 63778

CMD ["python", "app.py"]
