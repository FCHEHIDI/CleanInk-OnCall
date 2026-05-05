import { Component, ElementRef, ViewChild, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-landing',
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  template: `
    <div class="cioc-landing">
      <video #videoEl playsinline (ended)="goToLogin()">
        <source src="assets/videos/teaser.mp4" type="video/mp4" />
      </video>
      <button class="cioc-landing__skip" (click)="goToLogin()">Passer</button>
    </div>
  `,
  styles: [`
    .cioc-landing {
      position: fixed;
      top: 0; left: 0;
      width: 100vw; height: 100vh;
      background: #000;
      z-index: 9999;
      overflow: hidden;
    }
    .cioc-landing video {
      position: absolute;
      top: 0; left: 0;
      width: 100%; height: 100%;
      object-fit: cover;
    }
    .cioc-landing__skip {
      position: fixed;
      bottom: 2rem; right: 2rem;
      background: none;
      border: none;
      color: rgba(255,255,255,.35);
      font-size: .78rem;
      letter-spacing: .1em;
      text-transform: uppercase;
      cursor: pointer;
      padding: .4rem .6rem;
      z-index: 10000;
      transition: color .2s;
    }
    .cioc-landing__skip:hover { color: rgba(255,255,255,.7); }
  `],
})
export class LandingComponent implements AfterViewInit {
  @ViewChild('videoEl') videoEl!: ElementRef<HTMLVideoElement>;
  constructor(private router: Router) {}
  ngAfterViewInit(): void {
    const v = this.videoEl.nativeElement;
    v.muted = true;
    v.play().catch(() => this.goToLogin());
  }
  goToLogin(): void { this.router.navigate(['/auth/login']); }
}
