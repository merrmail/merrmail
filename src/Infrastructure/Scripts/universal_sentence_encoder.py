import os
import logging
import tensorflow as tf
import tensorflow_hub as hub

os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')


def load_or_download_universal_sentence_encoder(path):
    url = "https://tfhub.dev/google/universal-sentence-encoder/4"

    try:
        embed = tf.saved_model.load(path)
    except (OSError, tf.errors.NotFoundError):
        embed = hub.load(url)
        logging.info("Model downloaded from TensorFlow Hub.")

        tf.saved_model.save(embed, path)
        logging.info("Model saved to local path.")

    return embed


def calculate_cosine_similarity(embed, first, second):
    sentences = [first, second]
    embeddings = embed(sentences)
    first_embedding = embeddings[0]
    second_embedding = embeddings[1]

    try:
        cosine_similarity = tf.keras.losses.cosine_similarity(first_embedding, second_embedding).numpy()
        logging.info(f"Cosine Similarity between '{first}' and '{second}': {cosine_similarity}")
        return cosine_similarity.item()
    except Exception as e:
        logging.error(f"Error calculating cosine similarity: {e}")
        return None
