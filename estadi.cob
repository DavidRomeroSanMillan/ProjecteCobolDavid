       IDENTIFICATION DIVISION.
       PROGRAM-ID. ESTADI.

       ENVIRONMENT DIVISION.
       INPUT-OUTPUT SECTION.
       FILE-CONTROL.
           SELECT INPUT-FILE ASSIGN TO WS-INPUT-FILE
               ORGANIZATION IS LINE SEQUENTIAL.

       DATA DIVISION.
       FILE SECTION.
       FD  INPUT-FILE.
       01  INPUT-RECORD.
           05  REC-NOM         PIC X(30).
           05  REC-COST        PIC 9(8).
           05  REC-DATA        PIC X(10).
           05  REC-TIPUS       PIC X(20).

       WORKING-STORAGE SECTION.
       01  WS-INPUT-FILE       PIC X(50).
       01  WS-OUTPUT-FILE      PIC X(50).

       01  WS-CONTADOR         PIC 9(5) VALUE 0.
       01  WS-SUMA-TOTAL       PIC 9(10) VALUE 0.
       01  WS-MEDIA            PIC 9(8)V99 VALUE 0.
       01  WS-MEDIA-STR        PIC X(10).

       01  WS-EOF              PIC X VALUE 'N'.

       01  WS-TEMP-COST        PIC 9(8).
       01  WS-TEMP-TIPUS       PIC X(20).

       01  TOP3-TABLE.
           05  TOP3-ENTRY OCCURS 3 TIMES.
               10  TOP3-TIPUS      PIC X(20).
               10  TOP3-SUMA       PIC 9(10).
               10  TOP3-INDEX      PIC 9.

       01  WS-I                 PIC 9.
       01  WS-J                 PIC 9.
       01  WS-K                 PIC 9.
       01  WS-ENCONTRADO        PIC X.
       01  WS-SUMA-TIPUS        PIC 9(10).
       01  WS-TEMP-SWAPPED      PIC X.
       01  WS-TEMP-TIPOS-SUMA   PIC 9(10).
       01  WS-TEMP-TIPOS-TIPUS  PIC X(20).

       01  TIPOS-TABLE.
           05  TIPOS-ENTRY OCCURS 100 TIMES.
               10  TIPO-TIPUS      PIC X(20).
               10  TIPO-SUMA       PIC 9(10).
       01  WS-TIPOS-COUNT      PIC 9(3) VALUE 0.

       PROCEDURE DIVISION.
           ACCEPT WS-INPUT-FILE FROM COMMAND-LINE.
           MOVE FUNCTION TRIM(WS-INPUT-FILE) 
               TO WS-INPUT-FILE.

           MOVE FUNCTION CONCATENATE(
               FUNCTION TRIM(WS-INPUT-FILE),
               ".estad")
               TO WS-OUTPUT-FILE.

           PERFORM PROCESS-INPUT-FILE.

           IF WS-CONTADOR > 0
               COMPUTE WS-MEDIA ROUNDED = 
                   WS-SUMA-TOTAL / WS-CONTADOR
               MOVE WS-MEDIA TO WS-MEDIA-STR
           ELSE
               MOVE "0.00" TO WS-MEDIA-STR
           END-IF.

           PERFORM SORT-BY-TIPOS.

           PERFORM BUILD-TOP3.

           PERFORM WRITE-OUTPUT.

           STOP RUN.

       PROCESS-INPUT-FILE.
           OPEN INPUT INPUT-FILE.

           PERFORM UNTIL WS-EOF = 'Y'
               READ INPUT-FILE
                   AT END
                       MOVE 'Y' TO WS-EOF
                   NOT AT END
                       ADD 1 TO WS-CONTADOR
                       MOVE REC-COST TO WS-TEMP-COST
                       ADD WS-TEMP-COST TO WS-SUMA-TOTAL
                       MOVE REC-TIPUS TO WS-TEMP-TIPUS
                       PERFORM ADD-TIPO
               END-READ
           END-PERFORM.

           CLOSE INPUT-FILE.

       ADD-TIPO.
           MOVE 'N' TO WS-ENCONTRADO.
           PERFORM VARYING WS-I FROM 1 BY 1
               UNTIL WS-I > WS-TIPOS-COUNT
               IF TIPO-TIPUS(WS-I) = 
                   FUNCTION TRIM(WS-TEMP-TIPUS)
                   ADD WS-TEMP-COST TO TIPO-SUMA(WS-I)
                   MOVE 'Y' TO WS-ENCONTRADO
               END-IF
           END-PERFORM.

           IF WS-ENCONTRADO = 'N'
               ADD 1 TO WS-TIPOS-COUNT
               MOVE FUNCTION TRIM(WS-TEMP-TIPUS)
                   TO TIPO-TIPUS(WS-TIPOS-COUNT)
               MOVE WS-TEMP-COST TO TIPO-SUMA(WS-TIPOS-COUNT)
           END-IF.

       SORT-BY-TIPOS.
           MOVE 'Y' TO WS-TEMP-SWAPPED.
           PERFORM UNTIL WS-TEMP-SWAPPED = 'N'
               MOVE 'N' TO WS-TEMP-SWAPPED
               PERFORM VARYING WS-I FROM 1 BY 1
                   UNTIL WS-I >= WS-TIPOS-COUNT
                   COMPUTE WS-J = WS-I + 1
                   IF TIPO-SUMA(WS-I) < TIPO-SUMA(WS-J)
                       MOVE TIPO-TIPOS(WS-I) TO WS-TEMP-TIPOS-TIPUS
                       MOVE TIPO-SUMA(WS-I) TO WS-TEMP-TIPOS-SUMA
                       MOVE TIPO-TIPOS(WS-J) TO TIPO-TIPUS(WS-I)
                       MOVE TIPO-SUMA(WS-J) TO TIPO-SUMA(WS-I)
                       MOVE WS-TEMP-TIPOS-TIPUS TO TIPO-TIPUS(WS-J)
                       MOVE WS-TEMP-TIPOS-SUMA TO TIPO-SUMA(WS-J)
                       MOVE 'Y' TO WS-TEMP-SWAPPED
                   END-IF
               END-PERFORM
           END-PERFORM.

       BUILD-TOP3.
           PERFORM VARYING WS-I FROM 1 BY 1
               UNTIL WS-I > 3
               IF WS-I <= WS-TIPOS-COUNT
                   MOVE TIPO-TIPUS(WS-I) TO TOP3-TIPUS(WS-I)
                   MOVE TIPO-SUMA(WS-I) TO TOP3-SUMA(WS-I)
               ELSE
                   MOVE SPACES TO TOP3-TIPUS(WS-I)
                   MOVE 0 TO TOP3-SUMA(WS-I)
               END-IF
           END-PERFORM.

       WRITE-OUTPUT.
           OPEN OUTPUT OUTPUT-FILE.

           WRITE INPUT-RECORD FROM WS-MEDIA-STR.

           PERFORM VARYING WS-I FROM 1 BY 1
               UNTIL WS-I > 3
               MOVE FUNCTION CONCATENATE(
                   FUNCTION TRIM(TOP3-TIPUS(WS-I)),
                   "|",
                   FUNCTION TRIM(TOP3-SUMA(WS-I)))
                   TO INPUT-RECORD
               WRITE INPUT-RECORD
           END-PERFORM.

           CLOSE OUTPUT-FILE.
