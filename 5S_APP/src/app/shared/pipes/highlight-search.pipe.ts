import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Pipe({
  name: 'highlightSearch',
  standalone: true 
})
export class HighlightSearchPipe implements PipeTransform {

  constructor(private sanitizer: DomSanitizer) {}

  transform(text: string, searchKeyword: string): SafeHtml {
    if (!searchKeyword || !text) {
      return this.sanitizer.bypassSecurityTrustHtml(text);
    }

    const regex = new RegExp(`(${searchKeyword})`, 'gi');
    const highlightedText = text.replace(regex, `<span class="highlight-search">$1</span>`);
    return this.sanitizer.bypassSecurityTrustHtml(highlightedText);
  }
}