FROM tensorflow/tensorflow:latest

WORKDIR /app

RUN mkdir -p /universal_sentence_encoder
RUN pip install tensorflow-hub

COPY Scripts/app.py /app/app.py
COPY Scripts/universal_sentence_encoder.py /app/universal_sentence_encoder.py

EXPOSE 63778

CMD ["python", "app.py"]