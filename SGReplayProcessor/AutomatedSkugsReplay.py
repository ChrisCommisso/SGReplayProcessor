from http.server import BaseHTTPRequestHandler, HTTPServer
import time
import pyodbc
import vgamepad as vg
from pywinauto.findwindows    import find_window
from win32gui import SetForegroundWindow
import threading
import os

hostName = "localhost"
serverPort = 8080
currentIndex = 0
def navigateToReplayMenu():
    SetForegroundWindow(find_window(title='Skullgirls Encore'))
    time.sleep(2)
    #navigates to replay menu and starts the first replay
    gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()
    time.sleep(.1)
    gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()

    time.sleep(1)
    gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()
    time.sleep(.1)
    gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()

    time.sleep(1)
    gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()
    time.sleep(.1)
    gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()

    time.sleep(2)    
    gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_DPAD_UP)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()
    time.sleep(.1)
    gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_DPAD_UP)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()
    time.sleep(.2)
    gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()
    time.sleep(.1)
    gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()
    time.sleep(1)
    gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_DPAD_DOWN)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()
    time.sleep(.1)
    gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_DPAD_DOWN)
    x = threading.Thread(target=gamepad.update, args=())
    x.start()
    
    

def getUnsolvedData(cursor):
    unresolvedReplayData = []
    cursor.execute('SELECT * FROM replaydatabase.replaymessages WHERE resolved = 0;')
    for x in cursor:
        unresolvedReplayData.append(x)
    return unresolvedReplayData

#CURL REQUEST CMD COMMAND 
#curl --location --request GET localhost:8080 --form winner="Player1"
class MyServer(BaseHTTPRequestHandler):
    def do_GET(self):
        try:
            global currentIndex
            content_length = int(self.headers['Content-Length']) # <--- Gets the size of data
            post_data = self.rfile.read(content_length) # <--- Gets the data itself
            self.send_response(200)
            self.send_header("Content-type", "text/plain")
            self.end_headers()
            self.wfile.write(bytes("True", "utf-8"))
            post_data = post_data.decode('utf-8')
            if not(post_data.find("Player1") == -1):
                print("Player 1 won")
                currentCommand = "UPDATE replaydatabase.replaymessages SET resolved = 1, winner = '" + unresolvedReplayData[currentIndex][1] + "' WHERE submission = " + str(unresolvedReplayData[currentIndex][6]) + " AND id ='" + unresolvedReplayData[currentIndex][5] + "';"
                print(currentCommand)
                cursor.execute(currentCommand)
                connection.commit()
                currentIndex += 1
                try:
                    z = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.rnd", 'wb')
                    c = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.ini", 'w')
                    z.write(unresolvedReplayData[currentIndex][3])
                    z.close()
                    c.write(unresolvedReplayData[currentIndex][4])
                    c.close()
                    print("Overwrite complete.")
                    time.sleep(20)
                    SetForegroundWindow(find_window(title='Skullgirls Encore'))
                    gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                    time.sleep(.2)
                    gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                    time.sleep(.2)
                    gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                    time.sleep(.2)
                    gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                    time.sleep(10)
                    gamepad.right_trigger(value=0)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                    time.sleep(.2)
                    gamepad.right_trigger(value=255)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                    
                except:
                    print("No more to write.\n")
                
            if not(post_data.find("Player2") == -1):
                print("player2wins")
                currentCommand = "UPDATE replaydatabase.replaymessages SET resolved = 1 , winner = '" + str(unresolvedReplayData[currentIndex][2]) + "'WHERE submission = " + str(unresolvedReplayData[currentIndex][6]) + " AND id ='" + unresolvedReplayData[currentIndex][5] + "';"
                print(currentCommand)
                cursor.execute(currentCommand)
                connection.commit()
                currentIndex += 1
                try:
                    z = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.rnd", 'wb')
                    c = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.ini", 'w')
                    z.write(unresolvedReplayData[currentIndex][3])
                    z.close()
                    c.write(unresolvedReplayData[currentIndex][4])
                    c.close()
                    print("Overwrite complete.")
                    time.sleep(20)
                    SetForegroundWindow(find_window(title='Skullgirls Encore'))
                    gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                    time.sleep(.2)
                    gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                    time.sleep(.2)
                    gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                    time.sleep(.2)
                    gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                    time.sleep(.2)
                    gamepad.right_trigger(value=0)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                    time.sleep(10)
                    gamepad.right_trigger(value=255)
                    x = threading.Thread(target=gamepad.update, args=())
                    x.start()
                except:
                    print("No more to write...\n")
                if not(post_data.find("None") == -1):
                    print("Could Not Resolve. Going to next replay")
                    currentCommand = "UPDATE replaydatabase.replaymessages SET resolved = 1 , winner = 'Could Not Resolve' WHERE submission = " + str(unresolvedReplayData[currentIndex][6]) + " AND id ='" + unresolvedReplayData[currentIndex][5] + "';"
                    print(currentCommand)
                    cursor.execute(currentCommand)
                    connection.commit()
                    currentIndex += 1
                    try:
                        z = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.rnd", 'wb')
                        c = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.ini", 'w')
                        z.write(unresolvedReplayData[currentIndex][3])
                        z.close()
                        c.write(unresolvedReplayData[currentIndex][4])
                        c.close()
                        print("Overwrite complete.")
                        time.sleep(20)
                        SetForegroundWindow(find_window(title='Skullgirls Encore'))
                        gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                        x = threading.Thread(target=gamepad.update, args=())
                        x.start()
                        time.sleep(.2)
                        gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                        x = threading.Thread(target=gamepad.update, args=())
                        x.start()
                        time.sleep(.2)
                        gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                        x = threading.Thread(target=gamepad.update, args=())
                        x.start()
                        time.sleep(.2)
                        gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
                        x = threading.Thread(target=gamepad.update, args=())
                        x.start()
                        time.sleep(.2)
                        gamepad.right_trigger(value=0)
                        x = threading.Thread(target=gamepad.update, args=())
                        x.start()
                        time.sleep(10)
                        gamepad.right_trigger(value=255)
                        x = threading.Thread(target=gamepad.update, args=())
                        x.start()
                    except:
                        print("No more to write.\n")
        except Exception as err:
            print(err)
            self.wfile.write("POST request for {}".format(self.path).encode('utf-8'))
            self.send_response(500)
            self.send_header("Content-type", "text/plain")
            self.end_headers()
            self.wfile.write(bytes("False", "utf-8"))

    def do_POST(self):
        global unresolvedReplayData
        global currentIndex
        self.send_response(200)
        self.send_header("Content-type", "text/plain")
        self.end_headers()
        self.wfile.write(bytes("True", "utf-8"))
        unresolvedReplayData = getUnsolvedData(cursor)
        print("Fetched Data from SQL Server...\n")
        z = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.rnd", 'wb')
        c = open(r"C:\Users\tanne\OneDrive\Documents\Skullgirls\Replays_SG2EPlus\76561198132030993\round_0001.ini", 'w')
        z.write(unresolvedReplayData[currentIndex][3])
        z.close()
        c.write(unresolvedReplayData[currentIndex][4])
        c.close()
        print("Overwrite complete...\nOpening first replay\n")
        print("Invoking Win Finder C++ App")
        os.startfile(r".\SuperCoolCode")
        time.sleep(5)
        SetForegroundWindow(find_window(title='Skullgirls Encore'))
        time.sleep(1)
        gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
        x = threading.Thread(target=gamepad.update, args=())
        x.start()
        time.sleep(.2)
        gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
        x = threading.Thread(target=gamepad.update, args=())
        x.start()
        time.sleep(.2)
        gamepad.press_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
        x = threading.Thread(target=gamepad.update, args=())
        x.start()
        time.sleep(.2)
        gamepad.release_button(button=vg.XUSB_BUTTON.XUSB_GAMEPAD_A)
        x = threading.Thread(target=gamepad.update, args=())
        x.start()
        time.sleep(5)
        gamepad.right_trigger(value=255)
        x = threading.Thread(target=gamepad.update, args=())
        x.start()
        

        
        


if __name__ == "__main__":        
    gamepad = vg.VX360Gamepad()
    webServer = HTTPServer((hostName, serverPort), MyServer)
    connection = pyodbc.connect('driver=MySQL ODBC 8.0 Unicode Driver', host='localhost', database='replaydatabase', user='root', password='VictoryOverEvil8610')
    cursor = connection.cursor()
    try:
        #Instanciates a virtual controller object to naviagate the replay menus
        #Fetch unresolved entries in SQL DataBase before the main program loop
        #gets list of rows as lists (lists in lists)
        #main program loop
        print("Opening skullgirls...\n")
        os.startfile(r".\CoolAndProgramy")
        time.sleep(10)
        global unresolvedReplayData
        print("Navigating to replay menu...\n")
        navigateToReplayMenu()
        print("Done...\nWaiting for POST to start processes.....\n")
        while True:
            currentIndex = 0
            webServer.handle_request()
            for i in unresolvedReplayData:
                print("Replay Number ", currentIndex + 1, " Ready to go!\n")
                print("Server started http://%s:%s" % (hostName, serverPort))
                webServer.handle_request()
                print("Server stopped for database writing...\n")
            print("All replays processed successfully!\nWaiting for next POST request")
    except KeyboardInterrupt:
        pass
    
    webServer.server_close()
    print("Server stopped.")
