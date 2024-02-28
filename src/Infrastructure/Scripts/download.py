import os
import logging
import tensorflow as tf
import tensorflow_hub as hub

os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')

path = "/universal_sentence_encoder"
url = "https://tfhub.dev/google/universal-sentence-encoder/4"

embed = hub.load(url)
logging.info("Model downloaded from TensorFlow Hub")

tf.saved_model.save(embed, path)
logging.info(f"Model saved to {path}")
