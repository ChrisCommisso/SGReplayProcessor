import pyodbc

def makeBLOB(filePath):
    file = open(filePath, "rb")
    ablob = pyodbc.Binary(file.read())
    return ablob

def insertRNDFile(date, player1, player2, filePath, inidataPath, steamid, submissionNumber, isResolved, winner):
    ablob = makeBLOB(filePath)
    inidata = open(inidataPath, 'r').read()
    insert = """insert into replaymessages (replayCreated, player1, player2, rndData, iniData, id, submission, resolved, winner) values(?,?,?,?,?,?,?,?,?)"""
    binparams = (date, player1, player2, ablob, inidata, steamid, submissionNumber, isResolved, winner)
    cursor.execute(insert, binparams)
    connection.commit()
    print("BLOB commited to SQL server")

def overWrite(currentRNDFile, currentINIFile):
    rndfile = open(currentRNDFile, "rb")
    inifile = open(currentINIFile, 'r')
    rndToOverwrite = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.rnd", 'wb')
    iniToOverwrite = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.ini", 'w')
    x = rndfile.read()
    y = inifile.read()
    #killing myself tonight

    rndToOverwrite.write(bytes(x))
    iniToOverwrite.write(y)
    rndToOverwrite.close()
    rndfile.close()
    inifile.close()
    iniToOverwrite.close()
    print("File overwrite complete\n")

def getUnsolvedRNDData(cursor):
    unresolvedReplayData = []
    cursor.execute('SELECT * FROM replaydatabase.replaymessages WHERE resolved = 0;')
    for x in cursor:
        unresolvedReplayData.append(x[3])
    return unresolvedReplayData

def getUnsolvedINIData(cursor):
    unresolvedINIData = []
    cursor.execute('SELECT * FROM replaydatabase.replaymessages WHERE resolved = 0;')
    for x in cursor:
        unresolvedINIData.append(x[4])
    return unresolvedINIData

def getUnsolvedPlayer1Data(cursor):
    unresolvedP1Data = []
    cursor.execute('SELECT * FROM replaydatabase.replaymessages WHERE resolved = 0;')
    for x in cursor:
        unresolvedP1Data.append(x[1])
    return unresolvedP1Data

def getUnsolvedData(cursor):
    unresolvedReplayData = []
    cursor.execute('SELECT * FROM replaydatabase.replaymessages WHERE resolved = 0;')
    for x in cursor:
        unresolvedReplayData.append(x)
    return unresolvedReplayData


    
if __name__ == '__main__':
    print("Welcome to Themails' Python SQL Shell for ReplayDatabase!\n")
    connection = pyodbc.connect('driver=MySQL ODBC 8.0 Unicode Driver', host='localhost', database='replaydatabase', user='root', password='PooperScooper69')
    cursor = connection.cursor()
    

    while True:
        
        currentCommand = input("Input SQL Command to send to Database....\nType Q to quit.\n")
        if currentCommand == 'Q':
            break
        if currentCommand == "GETROWS":
            unresolvedReplayData = getUnsolvedData(cursor)
            print("Done...\n")
        if currentCommand == "INSERTRNDFILE":
            date = input("Date of submission?:\n")
            player1 = input("Player1 Name?:\n")
            player2 = input("Player2 Name?:\n")
            RNDfilePath = input(".rnd file path?:\n")
            inidata = input(".ini data path?:\n")
            steamid = input("steam id?:\n")
            submissionNumber = input("submission number?:\n")
            isResolved = input("is this replay entry resolved?(1 for TRUE, 0 for FALSE):\n")
            winner = input("winner name?(Press enter for no winner):\n")
            insertRNDFile(date, player1, player2, RNDfilePath, inidata, steamid, submissionNumber, isResolved, winner)
            print("Done...\n")
        if currentCommand == "OVERWRITE":
            print("rnd and ini paths given will be overwritten at replay_0001 and ini_0001")
            overWrite(input("Path of desired replay:\n"), input("Path of desired ini file:\n"))
            print("Done...\n")
        if currentCommand == "MAKEBLOB":
            print(makeBLOB(input("rnd file path: \n")))
            print("Done...\n")
        if currentCommand == "OVERWRITEFROMBLOB":
            z = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.rnd", 'wb')
            c = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.ini", 'w')
            z.write(unresolvedReplayData[0][3])
            z.close()
            c.write(unresolvedReplayData[0][4])
            c.close()
            print("Overwrite complete.")
    print("Goodbye.")