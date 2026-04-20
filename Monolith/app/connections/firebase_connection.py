import firebase_admin
from firebase_admin import credentials
from firebase_admin import credentials, firestore

# Esto es la parte hardcodeada, no supe como usar variables de entorno ya que pide un json :(
cred = credentials.Certificate("ruta al .json de las credenciales")
firebase_admin.initialize_app(cred)

db = firestore.client()