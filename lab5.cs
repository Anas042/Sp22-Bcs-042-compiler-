#include <stdio.h> 
#include <stdlib.h> 
#include <string.h> 
 
#define TABLE_SIZE 10  // Hash table size 
 
// Structure for a symbol table entry 
typedef struct Symbol { 
    char name[50];       // Identifier name 
    char type[20];       // Data type (e.g., int, float) 
    int scope;           // Scope level 
    struct Symbol *next; // Pointer for chaining (linked list) 
} Symbol; 
 
// Hash table (Array of pointers to Symbol nodes) 
Symbol *symbolTable[TABLE_SIZE]; 
 
// Hash function (Sum of ASCII values modulo table size) 
int hashFunction(char *name) { 
    int sum = 0; 
    for (int i = 0; name[i] != '\0'; i++) { 
        sum += name[i]; 
    } 
    return sum % TABLE_SIZE; 
} 
 
// Insert a symbol into the table 
void insertSymbol(char *name, char *type, int scope) { 
    int index = hashFunction(name); 
     
    // Create a new symbol node 
    Symbol *newSymbol = (Symbol *)malloc(sizeof(Symbol)); 
    strcpy(newSymbol->name, name); 
    strcpy(newSymbol->type, type); 
    newSymbol->scope = scope; 
    newSymbol->next = NULL; 
 
    // Insert at the beginning of the linked list (chaining) 
    if (symbolTable[index] == NULL) { 
        symbolTable[index] = newSymbol; 
    } else { 
        newSymbol->next = symbolTable[index]; 
        symbolTable[index] = newSymbol; 
    } 
    printf("Inserted: %s (%s, scope: %d)\n", name, type, scope); 
} 
 
// Search for a symbol in the table 
Symbol* searchSymbol(char *name) { 
    int index = hashFunction(name); 
    Symbol *temp = symbolTable[index]; 
 
    while (temp != NULL) { 
        if (strcmp(temp->name, name) == 0) { 
            return temp; // Found 
        } 
        temp = temp->next; 
    } 
    return NULL; // Not found 
} 
 
// Display the symbol table 
void displaySymbolTable() { 
    printf("\nSymbol Table:\n"); 
    printf("--------------------------------\n"); 
    printf("| Index | Name    | Type   | Scope |\n"); 
    printf("--------------------------------\n"); 
 
    for (int i = 0; i < TABLE_SIZE; i++) { 
        Symbol *temp = symbolTable[i]; 
        while (temp != NULL) { 
            printf("| %5d | %-7s | %-6s | %5d |\n", i, temp->name, temp->type, temp->scope); 
            temp = temp->next; 
        } 
    } 
    printf("--------------------------------\n"); 
} 
 
// Main function for testing 
int main() { 
    // Initializing the symbol table with NULL 
    for (int i = 0; i < TABLE_SIZE; i++) { 
        symbolTable[i] = NULL; 
    } 
 
    // Insert some symbols 
    insertSymbol("x", "int", 1); 
    insertSymbol("y", "float", 1); 
    insertSymbol("sum", "int", 2); 
    insertSymbol("product", "int", 2); 
    insertSymbol("y", "char", 3);  // Different scope 
 
    // Search for a symbol 
    char searchName[50]; 
    printf("\nEnter variable name to search: "); 
    scanf("%s", searchName); 
     
    Symbol *result = searchSymbol(searchName); 
    if (result) { 
        printf("Found: %s (%s, scope: %d)\n", result->name, result->type, result->scope); 
    } else { 
        printf("Symbol not found.\n"); 
    } 
 
    // Display the symbol table 
    displaySymbolTable(); 
 
    return 0; 
}
