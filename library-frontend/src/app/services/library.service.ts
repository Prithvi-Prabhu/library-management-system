import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Book } from '../models/book.model';
import { Member } from '../models/member.model';
import { Loan } from '../models/loan.model';

@Injectable({
  providedIn: 'root'
})
export class LibraryService {

  private baseUrl = 'https://localhost:7063/odata';

  constructor(private http: HttpClient) {}

  // ---------------- BOOKS ----------------

  getBooks(): Observable<{ value: Book[] }> {
    return this.http.get<{ value: Book[] }>(`${this.baseUrl}/Books`);
  }

  addBook(book: Book) {
    return this.http.post(`${this.baseUrl}/Books`, book);
  }

  updateBook(id: number, book: Partial<Book>) {
    return this.http.patch(`${this.baseUrl}/Books(${id})`, book);
  }

  deleteBook(id: number) {
    return this.http.delete(`${this.baseUrl}/Books(${id})`);
  }

  // ---------------- MEMBERS ----------------

  getMembers(): Observable<{ value: Member[] }> {
    return this.http.get<{ value: Member[] }>(`${this.baseUrl}/Members`);
  }

  addMember(member: Member) {
    return this.http.post(`${this.baseUrl}/Members`, member);
  }

  updateMember(id: number, member: Partial<Member>) {
    return this.http.patch(`${this.baseUrl}/Members(${id})`, member);
  }

  deleteMember(id: number) {
    return this.http.delete(`${this.baseUrl}/Members(${id})`);
  }

  // ---------------- LOANS ----------------

  getLoans(): Observable<{ value: Loan[] }> {
    return this.http.get<{ value: Loan[] }>(
      `${this.baseUrl}/Loans?$expand=Book,Member`
    );
  }

  createLoan(bookId: number, memberId: number) {
    return this.http.post(`${this.baseUrl}/Loans`, {
      BookId: bookId,
      MemberId: memberId,
      LoanDate: new Date()
    });
  }

returnLoan(id: number) {
  return this.http.patch(
    `${this.baseUrl}/Loans(${id})`,
    { Returned: true }
  );
}
}