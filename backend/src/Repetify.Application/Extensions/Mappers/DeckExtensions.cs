﻿using Repetify.Application.Dtos;
using Repetify.Domain.Entities;

namespace Repetify.Application.Extensions.Mappers;

public static class DeckExtensions
{
	public static DeckDto ToDto(this Deck deck)
	{
		ArgumentNullException.ThrowIfNull(deck);

		return new DeckDto(
			id: deck.Id,
			name: deck.Name,
			description: deck.Description,
			userId: deck.UserId,
			originalLanguage: deck.OriginalLanguage,
			translatedLanguage: deck.TranslatedLanguage
		);
	}

	public static IEnumerable<DeckDto> ToDtoList(this IEnumerable<Deck> decks)
	{		return decks is null ? throw new ArgumentNullException(nameof(decks)) : decks.Select(deck => deck.ToDto());
	}

	public static Deck ToEntity(this AddOrUpdateDeckDto deckDto, Guid userId, Guid? deckId = null)
	{
		ArgumentNullException.ThrowIfNull(deckDto);

		return new(
				id: deckId,
				name: deckDto.Name!,
				description: deckDto.Description,
				userId: userId,
				originalLanguage: deckDto.OriginalLanguage!,
				translatedLanguage: deckDto.TranslatedLanguage!
			);
	}
}
