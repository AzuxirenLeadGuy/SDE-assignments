import pymongo                  # pip install pymongo && pip install dnspython
from pandas import DataFrame    # pip install pandas
client = pymongo.MongoClient("mongodb+srv://iitj_free:abcd@cluster0.i53a4.mongodb.net/LibraryDB?retryWrites=true&w=majority")
df = DataFrame(client['LibraryDB']['Books'].find())
print(df)