import os
import logging
import tensorflow as tf

os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')


def load_universal_sentence_encoder(path):
    embed = tf.saved_model.load(path)
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
