import tensorflow as tf


def load_universal_sentence_encoder(path):
    embed = tf.saved_model.load(path)
    return embed


def calculate_cosine_similarity(embed, first, second):
    sentences = [first, second]
    embeddings = embed(sentences)
    first_embedding = embeddings[0]
    second_embedding = embeddings[1]
    cosine_similarity = tf.keras.losses.cosine_similarity(first_embedding, second_embedding).numpy()
    print(f"Cosine Similarity between '{first}' and '{second}': {cosine_similarity}")
    return cosine_similarity.item()
