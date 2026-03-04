import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LibraryService } from './services/library.service';
import { Book } from './models/book.model';
import { Member } from './models/member.model';
import { Loan } from './models/loan.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <div style="padding:20px; font-family: Arial">

    <h1>📚 Library Management</h1>

    <!-- ================= BOOKS ================= -->

    <h2>Books</h2>

    <form (ngSubmit)="saveBook()" style="margin-bottom:10px;">
      <input placeholder="Title" [(ngModel)]="currentBook.Title" name="title">
      <input placeholder="Author" [(ngModel)]="currentBook.Author" name="author">
      <input placeholder="Genre" [(ngModel)]="currentBook.Genre" name="genre">
      <input type="number" placeholder="Year" [(ngModel)]="currentBook.YearPublished" name="year">
      <input type="number" placeholder="Copies" [(ngModel)]="currentBook.AvailableCopies" name="copies">

      <button type="submit">
        {{ currentBook.Id ? 'Update' : 'Add' }}
      </button>
    </form>

    <table border="1" cellpadding="6">
      <tr>
        <th>Title</th>
        <th>Author</th>
        <th>Copies</th>
        <th>Actions</th>
      </tr>

      <tr *ngFor="let book of books">
        <td>{{ book.Title }}</td>
        <td>{{ book.Author }}</td>
        <td>{{ book.AvailableCopies }}</td>
        <td>
          <button (click)="editBook(book)">Edit</button>
          <button (click)="deleteBook(book.Id)">Delete</button>
        </td>
      </tr>
    </table>

    <hr>

    <!-- ================= MEMBERS ================= -->

    <h2>Members</h2>

    <form (ngSubmit)="saveMember()" style="margin-bottom:10px;">
      <input placeholder="Name" [(ngModel)]="currentMember.Name" name="mname">
      <input placeholder="Email" [(ngModel)]="currentMember.Email" name="memail">

      <button type="submit">
        {{ currentMember.Id ? 'Update' : 'Add' }}
      </button>
    </form>

    <table border="1" cellpadding="6">
      <tr>
        <th>Name</th>
        <th>Email</th>
        <th>Actions</th>
      </tr>

      <tr *ngFor="let member of members">
        <td>{{ member.Name }}</td>
        <td>{{ member.Email }}</td>
        <td>
          <button (click)="editMember(member)">Edit</button>
          <button (click)="deleteMember(member.Id)">Delete</button>
        </td>
      </tr>
    </table>

    <hr>

    <!-- ================= CREATE LOAN ================= -->

    <h2>Create Loan</h2>

    <select [(ngModel)]="selectedBookId" name="book">
      <option [ngValue]="null">Select Book</option>
      <option *ngFor="let book of books" [ngValue]="book.Id">
        {{ book.Title }} ({{ book.AvailableCopies }} available)
      </option>
    </select>

    <select [(ngModel)]="selectedMemberId" name="member">
      <option [ngValue]="null">Select Member</option>
      <option *ngFor="let member of members" [ngValue]="member.Id">
        {{ member.Name }}
      </option>
    </select>

    <button (click)="borrow()">Borrow</button>

    <hr>

    <!-- ================= LOANS ================= -->

    <h2>Active Loans</h2>

<table border="1" cellpadding="6">
<tr>
  <th>Book</th>
  <th>Member</th>
  <th>Loan Date</th>
  <th>Due Date</th>
  <th>Status</th>
  <th>Action</th>
</tr>

<tr *ngFor="let loan of loans">

  <td>{{ loan.Book?.Title }}</td>
  <td>{{ loan.Member?.Name }}</td>

  <td>{{ loan.LoanDate | date:'medium' }}</td>
  <td>{{ loan.DueDate | date:'mediumDate' }}</td>

  <!-- STATUS -->
  <td>
    <span *ngIf="loan.Returned; else dueStatus">
      Returned
    </span>

    <ng-template #dueStatus>
      Due
    </ng-template>
  </td>

  <!-- ACTION -->
  <td>
    <button 
      *ngIf="!loan.Returned"
      (click)="returnBook(loan.Id)">
      Return
    </button>
  </td>

</tr>
</table>

  </div>
  `
})
export class App implements OnInit {

  books: Book[] = [];
  members: Member[] = [];
  loans: Loan[] = [];

  currentBook: any = {};
  currentMember: any = {};

  selectedBookId!: number | null;
  selectedMemberId!: number | null;

  constructor(private libraryService: LibraryService) {}

  ngOnInit(): void {
    this.refresh();
  }

  refresh() {
    this.libraryService.getBooks().subscribe(res => this.books = res.value);
    this.libraryService.getMembers().subscribe(res => this.members = res.value);
    this.libraryService.getLoans().subscribe(res => this.loans = res.value);
  }

  // ---------- BOOKS ----------
  saveBook() {
    if (this.currentBook.Id) {
      this.libraryService.updateBook(this.currentBook.Id, this.currentBook)
        .subscribe(() => this.refresh());
    } else {
      this.libraryService.addBook(this.currentBook)
        .subscribe(() => this.refresh());
    }
    this.currentBook = {};
  }

  editBook(book: Book) {
    this.currentBook = { ...book };
  }

  deleteBook(id: number) {
    this.libraryService.deleteBook(id)
      .subscribe(() => this.refresh());
  }

  // ---------- MEMBERS ----------
  saveMember() {
    if (this.currentMember.Id) {
      this.libraryService.updateMember(this.currentMember.Id, this.currentMember)
        .subscribe(() => this.refresh());
    } else {
      this.libraryService.addMember(this.currentMember)
        .subscribe(() => this.refresh());
    }
    this.currentMember = {};
  }

  editMember(member: Member) {
    this.currentMember = { ...member };
  }

  deleteMember(id: number) {
    this.libraryService.deleteMember(id)
      .subscribe(() => this.refresh());
  }

  // ---------- LOANS ----------
  borrow() {
    if (!this.selectedBookId || !this.selectedMemberId) return;

    this.libraryService.createLoan(this.selectedBookId, this.selectedMemberId)
      .subscribe(() => this.refresh());

    this.selectedBookId = null;
    this.selectedMemberId = null;
  }

  returnBook(id: number) {
  this.libraryService.returnLoan(id)
    .subscribe(() => {
      this.refresh(); // reload loans after update
    });
}
}